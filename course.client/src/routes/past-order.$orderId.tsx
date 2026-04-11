import api from '#/api';
import Star from '#/components/assets/star';
import EOrderRecordStatus, { orderRecordStatusMap } from '#/models/orderStatus';
import InventoryRecordServer from '#/models/server/inventoryRecordServer';
import OrderInfo from '#/models/server/requests/orderInfo';
import { replaceRouteParams } from '#/utils/http';
import { useQueries } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import axios from 'axios';
import { useMemo } from 'react';

export const Route = createFileRoute('/past-order/$orderId')({
  component: RouteComponent,
  loader: ({ params }) =>
    axios.get(replaceRouteParams(`/${api.client.order.get}`, { id: params.orderId })),
  wrapInSuspense: true,
});

function RouteComponent() {
  const orderRequest = Route.useLoaderData();
  const order = orderRequest.data as OrderInfo;

  const inventoryRecordsQuery = useQueries({
    queries: (() => {
      const queries = new Set<{
        queryKey: (string | number)[];
        queryFn: () => Promise<InventoryRecordServer>;
      }>();

      Object.entries(order.orderRecords).forEach(([_id, orderRecord]) => {
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
      });

      return [...queries];
    })(),
  });
  const inventoryMap = useMemo(() => {
    if (!inventoryRecordsQuery.some((q) => !!q.data)) return null;

    return inventoryRecordsQuery
      .map((q) =>
        !!q.data
          ? {
              inventoryRecordId: q.data.id,
              image: q.data.image,
              size: q.data.size,
            }
          : null,
      )
      .filter((item) => item !== null);
  }, [inventoryRecordsQuery]);

  const handleRatingClick = (orderRecordId: number, rating: number) => {
    console.log(rating);
  }

  return (
    <div className="gap-4 page">
      <h2 className='font-semibold'>Детали заказа #{order.id} от {new Date(order.date).toLocaleDateString('ru')}</h2>
      {Object.entries(order.orderRecords).map(([id, record]) => {
        const inventoryRecord = inventoryMap?.find(
          (i) => i?.inventoryRecordId === record.inventoryRecordId,
        );

        return (
          <div key={id} className="flex flex-row gap-8 bg-slate-900 p-4 rounded-lg justify-between">
            <div className="flex flex-col gap-2">
              <p className="font-semibold">{record.productTitle}</p>
              <div className="flex flex-row gap-2 items-center">
                <div className="bg-slate-700 w-fit py-1 px-3 rounded-md border-solid border border-gray-700">
                  {record.productVariation}
                </div>
                {inventoryRecord?.size && (
                  <div className="bg-slate-700 w-fit py-1 px-3 rounded-md border-solid border border-gray-700">
                    {inventoryRecord.size}
                  </div>
                )}
                <p>{record.quantity} шт.</p>
              </div>
              <p>{orderRecordStatusMap[record.status]}</p>
              {record.status === EOrderRecordStatus.Done && (
                <div className="flex flex-row-reverse w-fit">
                  {Array.from({ length: 5 }, (_, i) => (
                    <button key={i} onClick={() => handleRatingClick(record.id, 5 - i)} className='order-rating-star'>
                      <Star className="size-6" />
                    </button>
                  ))}
                </div>
              )}
            </div>
            {inventoryRecord?.image && (
              <img
                key={record.inventoryRecordId}
                className={
                  'aspect-square object-cover rounded-md w-40 hover:object-contain bg-gray-500'
                }
                src={`data:image/*;base64,${inventoryRecord.image}`}
              />
            )}
          </div>
        );
      })}
    </div>
  );
}
