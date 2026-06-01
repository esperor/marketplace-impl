import { useMutation, useQueries, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import axios from 'axios';
import api from '../api';
import OrderInfo from '../models/server/requests/orderInfo';
import EOrderRecordStatus from '../models/orderStatus';
import { replaceRouteParams } from '../utils/http';
import InventoryRecordServer from '#/models/server/inventoryRecordServer';
import { useMemo } from 'react';
import OrderRecordInfoModel from '#/models/server/orderRecordInfoModel';
import { Link } from '@tanstack/react-router';

const g_addressPropertiesMap = {
  city: 'Город',
  street: 'Улица',
  building: 'Дом',
  entrance: 'Подъезд',
  floor: 'Этаж',
  flat: 'Квартира/Офис',
};

function Orders() {
  const queryClient = useQueryClient();
  const ordersQuery = useSuspenseQuery<OrderInfo[]>(
    {
      queryKey: ['user-orders'],
      queryFn: async () => {
        const res = await axios.get(`/${api.client.order.getAll}`);
        return res.data;
      },
    },
    queryClient,
  );
  const inventoryRecordsQuery = useQueries({
    queries: (() => {
      const queries = new Set<{
        queryKey: (string | number)[];
        queryFn: () => Promise<InventoryRecordServer>;
      }>();
      const processedInventoryRecordsIds = new Set<number>();

      ordersQuery.data.forEach((order) =>
        Object.entries(order.orderRecords).forEach(([_id, orderRecord]) => {
          if ([...processedInventoryRecordsIds].includes(orderRecord.inventoryRecordId)) return;

          const query = {
            queryKey: ['inventory-record', orderRecord.inventoryRecordId],
            queryFn: async (): Promise<InventoryRecordServer> => {
              const res = await axios.get(
                replaceRouteParams(`/${api.public.inventory.get}`, {
                  id: orderRecord.inventoryRecordId,
                }),
              );
              return res.data;
            },
          };
          queries.add(query);
          processedInventoryRecordsIds.add(orderRecord.inventoryRecordId);
        }),
      );

      return [...queries];
    })(),
  });
  const imageMap = useMemo(() => {
    if (!inventoryRecordsQuery.some((q) => !!q.data)) return null;

    return inventoryRecordsQuery
      .map((q) =>
        !!q.data
          ? {
              inventoryRecordId: q.data.id,
              image: q.data.image,
            }
          : null,
      )
      .filter((item) => item !== null);
  }, [inventoryRecordsQuery]);
  const cancel = useMutation({
    mutationFn: async (id: number) => {
      await axios.put(replaceRouteParams(`/${api.client.order.cancel}`, { id: id }));
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user-orders'] });
    },
  });

  const handleCancel = async (id: number) => {
    await cancel.mutateAsync(id);
  };

  if (ordersQuery.isError) return <p>Произошла ошибка: {ordersQuery.error.message}</p>;

  if (ordersQuery.data.length == 0) return <p>Заказов нет</p>;

  const getOrderImages = (orderRecords: Record<number, OrderRecordInfoModel>) => {
    if (imageMap === null) return [];
    const imageSet = new Set<(typeof imageMap)[number]>();

    Object.entries(orderRecords).forEach(([_, orderRecord]) => {
      const imageMapEntry = imageMap.find(
        (item) => item.inventoryRecordId === orderRecord.inventoryRecordId,
      );
      if (!!imageMapEntry && !!imageMapEntry.image) {
        imageSet.add(imageMapEntry);
      }
    });

    return [...imageSet];
  };

  const getOrderImagesStyles = (
    index: number,
    imagesAmount: number,
  ): { className: string; style: React.CSSProperties } => {
    const imageSideRem = 10; // sync with className (10 rem = h-40)
    const imageMargin = imageSideRem / imagesAmount;
    const className = `aspect-square object-cover rounded-md max-h-40 ${
      index > 0 ? 'absolute top-0' : ''
    }`;
    const style: React.CSSProperties = {
      left: `calc(${imageMargin}rem * ${index})`,
      ...(index === 0 ? { marginRight: `calc(${imageMargin}rem * ${imagesAmount - 1})` } : {}),
      ...(index !== 0 ? { boxShadow: `-2px 0px 7px rgb(0, 0, 0, 0.6)` } : {}),
      scale: `calc(${imagesAmount - index} * 5 / 100%)`,
    };
    return { className, style };
  };

  const getAddressJsx = (order: OrderInfo) => {
    try {
      const parsedAddress = JSON.parse(order.address);
      return (
        <div className='rounded-md bg-gray-900 px-2 py-1 w-fit'>
          <h4 className='font-semibold'>Адрес</h4>
          <div className="px-6">
          {Object.keys(parsedAddress).map((key, i) => (
            <p key={i}>
              {g_addressPropertiesMap[key as keyof typeof g_addressPropertiesMap]}:{' '}
              {parsedAddress[key]}
            </p>
          ))}
          </div>
        </div>
      );
    } catch (e) {
      console.error(e);
      return order.address;
    }
  };

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-row justify-between">
        <h2 className="py-2">Ваши заказы:</h2>
        <button
          type="button"
          className="btn"
          onClick={() => {
            queryClient.invalidateQueries({ queryKey: ['user-orders'] });
          }}
        >
          Обновить
        </button>
      </div>
      <div className="w-full h-fit max-h-[70vh] overflow-y-auto gap-[0.5rem] pr-1 flex flex-col">
        {ordersQuery.data?.map((order) => {
          if (Object.values(order.orderRecords).length === 0) return null;

          const orderStatus = Object.values(order.orderRecords)
            .map((r) => r.status)
            .reduce<EOrderRecordStatus | null>((prev, cur) => {
              if (prev === null) return cur;
              return prev < cur ? prev : cur;
            }, null)!;

          const isRateRecordButtonShown = Object.values(order.orderRecords).some(
            (v) => v.status === EOrderRecordStatus.Done && typeof v.ratingValue !== 'number',
          );

          return (
            <div
              key={order.id}
              className="flex flex-col gap-6 bg-slate-950 p-4 rounded-lg shadow-lg flex-[0_0_calc(50%-0.5rem)]"
            >
              <div className="flex flex-row gap-6 ">
                <div className="flex flex-col flex-1" key={order.id}>
                  <h3>{`#${order.id}`}</h3>
                  <p>{getAddressJsx(order)}</p>
                  <p>{new Date(order.date).toLocaleDateString('ru')}</p>
                  <p>{`Стоимость: ${order.totalPrice} руб.`}</p>
                </div>
                <div className="relative">
                  {getOrderImages(order.orderRecords).map(
                    ({ inventoryRecordId, image: imageData }, i, arr) => {
                      const { className, style } = getOrderImagesStyles(i, arr.length);

                      return (
                        <img
                          key={inventoryRecordId}
                          className={className}
                          style={style}
                          src={`data:image/*;base64,${imageData}`}
                        />
                      );
                    },
                  )}
                </div>
              </div>
              <div className="flex flex-row justify-between mt-auto">
                <Link
                  from={'/identity/profile'}
                  to={'/past-order/$orderId'}
                  params={{ orderId: order.id.toString() }}
                  className={`mr-auto btn btn--highlight ${isRateRecordButtonShown ? '' : 'hidden'}`}
                >
                  Оценить покупки
                </Link>
                <button
                  type="button"
                  onClick={() => handleCancel(order.id)}
                  className="ml-auto btn"
                  disabled={
                    orderStatus === EOrderRecordStatus.Canceled ||
                    orderStatus === EOrderRecordStatus.Done
                  }
                >
                  Отменить
                </button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

export default Orders;
