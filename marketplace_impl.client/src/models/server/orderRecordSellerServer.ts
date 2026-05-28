import OrderRecordInfoModel from "./orderRecordInfoModel";

export default interface OrderRecordSellerServer extends OrderRecordInfoModel {
  date: string;
  price: number;
  storeId: number;
  storeName: string;
  delivererContactInfo: string;
  delivererName: string;
}