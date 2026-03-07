import Reset from '#/components/assets/reset';
import constant from '#/utils/constants';
import OrderRecordsFiltersModel from '#/models/orderRecordsFiltersModel';
import { orderRecordStatusMap } from '#/models/orderStatus';

export default function OrderFilters({
  filters,
  onLimitChange,
  setFilters,
  onInvalidate,
}: {
  filters: OrderRecordsFiltersModel;
  onLimitChange: () => void;
  setFilters: (filters: OrderRecordsFiltersModel) => void;
  onInvalidate: () => void;
}) {
  return (
    <div className="flex flex-row gap-4 flex-wrap sm:flex-nowrap">
      <div className="flex flex-row flex-wrap items-center gap-x-6 gap-y-4">
        <div className="flex flex-row gap-4">
          <label htmlFor="filters.status">Статус:</label>
          <select
            className="text-gray-900 h-fit"
            id="filters.status"
            value={filters.status ?? ''}
            onChange={(e) => setFilters({ ...filters, status: e.target.value !== '' ? parseInt(e.target.value) : null })}
          >
            <option key={'null'} value={''}>{'Любой'}</option>
            {Object.entries(orderRecordStatusMap).map(([key, value]) => (
              <option key={key} value={key}>
                {value}
              </option>
            ))}
          </select>
        </div>
        <div className="flex flex-row gap-4">
          <label htmlFor="filters.limit" className="text-nowrap">
            Показывать заказов:
          </label>
          <input
            type="number"
            className="w-16 h-fit"
            value={filters.limit}
            onChange={(e) => setFilters({ ...filters, limit: parseInt(e.target.value) })}
            onBlur={() =>
              setTimeout(() => {
                onInvalidate();
                onLimitChange();
              }, 50)
            }
          />
          <button
            className="btn p-0 bg-transparent"
            title="Сбросить количество показываемых товаров"
            onClick={() => {
              setFilters({ ...filters, limit: constant.defaultLimit });
              setTimeout(() => {
                onInvalidate();
                onLimitChange();
              }, 50);
            }}
          >
            <Reset />
          </button>
        </div>
      </div>
      <div className="flex flex-row gap-4 ml-auto pl-6">
        <button
          type="button"
          className="btn h-fit"
          title="Применить фильтры"
          onClick={() => {
            onInvalidate();
            onLimitChange();
          }}
        >
          Применить
        </button>
        <button
          className="h-fit btn px-0 bg-transparent"
          onClick={() => {
            setFilters({
              ...filters,
              status: null,
            });
            setTimeout(onInvalidate, 50);
          }}
        >
          <Reset />
        </button>
      </div>
    </div>
  );
}
