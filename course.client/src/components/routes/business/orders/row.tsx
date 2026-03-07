import { orderRecordStatusMap } from '#/models/orderStatus';
import OrderRecordSellerServer from '#/models/server/orderRecordSellerServer';

export default function Row({
  record,
  onSelect,
  isSelected,
}: {
  record: OrderRecordSellerServer;
  onSelect?: (id: number) => void;
  isSelected: boolean;
}) {
  return (
    <tr onClick={() => onSelect?.(record.id)} className={isSelected ? '!bg-yellow-900' : ''}>
      <td>
        <div
          className={`rounded-full ${isSelected ? 'bg-white' : 'bg-transparent'} aspect-square self-center w-2`}
        ></div>
      </td>
      <td>{new Date(record.date).toLocaleDateString('ru')}</td>
      <td>{orderRecordStatusMap[record.status]}</td>
      <td>{record.storeName}</td>
      <td>{record.price * record.quantity}</td>
      <td>{record.delivererName !== null ? 'Да' : 'Нет'}</td>
    </tr>
  );
}
