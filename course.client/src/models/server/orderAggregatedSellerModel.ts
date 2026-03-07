import OrderRecordInfoModel from "./orderRecordInfoModel";

export default interface OrderAggregatedSellerModel {
  id: number;
  totalPrice: number;
  date: Date;
  orderRecords: OrderRecordInfoModel[];
  delivererName?: string;
  delivererContactInfo?: string;
}