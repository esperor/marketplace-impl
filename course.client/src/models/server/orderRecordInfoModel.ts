import EOrderRecordStatus from "../orderStatus";

export default interface OrderRecordInfoModel {
  id: number;
  orderId: number;
  inventoryRecordId: number;
  quantity: number;
  status: EOrderRecordStatus;
  productTitle: string;
  productVariation: string;
}