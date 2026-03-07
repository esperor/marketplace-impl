import EOrderRecordStatus from "../../orderStatus";

export default interface OrderInfo {
  id: number;
  userId: number;
  address: string;
  totalPrice: number;
  date: string;
  status: EOrderRecordStatus;
  orderedRecords: Record<number, number>;
}