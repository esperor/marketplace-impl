import EOrderRecordStatus from "../orderStatus";

export default interface OrderRecordSellerServer {
    id: number;
    orderId: number;
    inventoryRecordId: number;
    date: string;
    status: EOrderRecordStatus;
    quantity: number;
    price: number;
    storeId: number;
    storeName: string;
    delivererContactInfo: string;
    delivererName: string;
}