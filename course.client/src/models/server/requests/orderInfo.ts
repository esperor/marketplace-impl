import OrderRecordInfoModel from "../orderRecordInfoModel";

export default interface OrderInfo {
  id: number;
  userId: number;
  address: string;
  totalPrice: number;
  date: string;
  orderRecords: Record<number, OrderRecordInfoModel>;
}