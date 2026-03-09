import api from '#/api';
import StatusUpdateButton from '#/components/routes/business/order.orderId/statusUpdateButton';
import InventoryRecord from '#/models/inventoryRecord';
import { orderRecordStatusMap } from '#/models/orderStatus';
import OrderAggregatedSellerModel from '#/models/server/orderAggregatedSellerModel';
import { authenticateSeller, replaceRouteParams } from '#/utils/http';
import randomStock from '#/utils/randomStock';
import { useQuery, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import axios from 'axios';
import { useState } from 'react';

export const Route = createFileRoute('/business/order/$orderId')({
  component: ProcessOrder,
  beforeLoad: authenticateSeller,
});

function ProcessOrder() {
  const pathParams = Route.useParams();
  const queryClient = useQueryClient();
  const orderQuery = useSuspenseQuery<OrderAggregatedSellerModel>(
    {
      queryKey: ['seller-order', pathParams.orderId],
      queryFn: async () => {
        const res = await axios.get(
          replaceRouteParams(`/${api.business.order.get}`, { id: pathParams.orderId }),
        );
        return res.data;
      },
    },
    queryClient,
  );
  if (orderQuery.error) throw orderQuery.error;
  const [selectedRecordId, setSelectedRecordId] = useState<number>(
    orderQuery.data.orderRecords.at(0)!.id,
  );
  const selectedRecord = orderQuery.data.orderRecords.find((r) => r.id === selectedRecordId)!;
  const selectedRecordInventoryId = selectedRecord.inventoryRecordId;
  const inventoryQuery = useQuery<InventoryRecord>(
    {
      queryKey: ['inventory', selectedRecordInventoryId],
      queryFn: async () => {
        const res = await axios.get(
          replaceRouteParams(`/${api.business.inventory.get}`, { id: selectedRecordInventoryId }),
        );
        return res.data;
      },
    },
    queryClient,
  );

  return (
    <div className="page gap-10 flex-col">
      <div className="flex flex-col">
        <h3 className="text-2xl pb-4">
          Заказ №{orderQuery.data.id} от {new Date(orderQuery.data.date)?.toLocaleDateString('ru')}
        </h3>
        <p>
          Назначенный доставщик:{' '}
          {orderQuery.data.delivererName
            ? `${orderQuery.data.delivererName} (${orderQuery.data.delivererContactInfo})`
            : 'отсутствует'}
        </p>
        <p>Стоимость: {orderQuery.data.totalPrice} руб.</p>
        <p>Комиссия 15%: {Math.floor(orderQuery.data.totalPrice * 0.15)} руб.</p>
        <p>Итого ваша выручка: {Math.ceil(orderQuery.data.totalPrice * 0.85)} руб.</p>
      </div>
      <div className="flex flex-row gap-4">
        <table className="table w-1/2 h-fit">
          <thead>
            <tr className="bg-slate-600">
              <th></th>
              <th>Название товара</th>
              <th>Количество</th>
              <th>Статус</th>
            </tr>
          </thead>
          <tbody>
            {orderQuery.data.orderRecords.map((record) => {
              const isSelected = record.id === selectedRecordId;
              return (
                <tr
                  key={record.id}
                  onClick={() => setSelectedRecordId(record.id)}
                  className={isSelected ? '!bg-yellow-900' : ''}
                >
                  <td>
                    <div
                      className={`rounded-full ${isSelected ? 'bg-white' : 'bg-transparent'} aspect-square self-center w-2`}
                    ></div>
                  </td>
                  <td>
                    {record.productTitle} ({record.productVariation})
                  </td>
                  <td>{record.quantity}</td>
                  <td>{orderRecordStatusMap[record.status]}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
        <div className="flex flex-col gap-4 w-1/2">
          {inventoryQuery.data ? (
            <>
              <div className="max-h-[40dvh] z-[0] items-center flex flex-col justify-center w-fit">
                <img
                  className="rounded-lg object-contain max-w-full max-h-full w-fit h-fit"
                  src={
                    inventoryQuery.data.image
                      ? `data:image/*;base64,${inventoryQuery.data.image}`
                      : `/stock/${randomStock()}.jpg`
                  }
                  alt={selectedRecord.productTitle}
                />
              </div>
              <h4 className="text-xl mr-auto">{selectedRecord.productTitle}</h4>
              <div className="flex flex-col">
                <p>Вариант: {inventoryQuery.data.variation}</p>
                {inventoryQuery.data.size && <p>Размер: {inventoryQuery.data.size}</p>}
                <p>Цена: {inventoryQuery.data.price} руб.</p>
                <p>Заказано: {selectedRecord.quantity} шт.</p>
              </div>
              <StatusUpdateButton
                selectedRecordId={selectedRecordId}
                selectedRecordStatus={selectedRecord.status}
                queryClient={queryClient}
              />
            </>
          ) : (
            'Загрузка...'
          )}
        </div>
      </div>
    </div>
  );
}
