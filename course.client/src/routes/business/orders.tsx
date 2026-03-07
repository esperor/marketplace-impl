import OrderFilters from '#/components/routes/business/orders/orderFilters';
import Row from '#/components/routes/business/orders/row';
import useOrders from '#/hooks/useOrders';
import { authenticateSeller } from '#/utils/http'
import { createFileRoute, Link, useSearch } from '@tanstack/react-router'
import { useState } from 'react';

export const Route = createFileRoute('/business/orders')({
  component: Orders,
  beforeLoad: authenticateSeller,
})

function Orders() {
  const searchParams = useSearch({ from: '/business/orders' });
  const {
    filters,
    setFilters,
    resetInfiniteQuery,
    data,
    error,
    status,
    queryClient,
    LoadMoreBtn,
  } = useOrders(searchParams);
  const [selectedRecordId, setSelectedRecordId] = useState<number | null>(null);

  const isDataEmpty = !!data ? data.pages.flat().length == 0 : null;

  if (status === 'pending') return <div>Загрузка...</div>;
  if (status === 'error') return <div>{error?.message}</div>;

  return <div className="page gap-4">
    <OrderFilters
      filters={filters}
      setFilters={setFilters}
      onLimitChange={resetInfiniteQuery}
      onInvalidate={() => queryClient.invalidateQueries({ queryKey: ['products'] })}
    />
    <div className="flex flex-row flex-wrap">
      <Link
        from="/business/orders"
        to="/business/order/$orderId"
        params={{ orderId: selectedRecordId?.toString() ?? '' }}
        className="btn"
        disabled={!selectedRecordId}
      >
        Открыть детали
      </Link>
    </div>
    <table className="table">
      <thead>
        <tr className="bg-slate-600">
          <th></th>
          <th>Дата</th>
          <th>Статус</th>
          <th>Магазин</th>
          <th>Сумма</th>
          <th>Доставщик назначен</th>
        </tr>
      </thead>
      <tbody>
        {data?.pages.map((page) => page.map((orderRecord) => (
          <Row
            key={orderRecord.id}
            record={orderRecord}
            onSelect={setSelectedRecordId}
            isSelected={orderRecord.id === selectedRecordId}
          />
        )))}
      </tbody>
    </table>

    {isDataEmpty && <div className="self-center">Ничего не нашлось</div>}
    {isDataEmpty === false && (
      <div className="w-full flex">
        <LoadMoreBtn />
      </div>
    )}
  </div>
}
