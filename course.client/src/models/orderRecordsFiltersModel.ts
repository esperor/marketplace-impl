import EOrderRecordStatus from "./orderStatus";

interface OrderRecordsFiltersModel {
  limit: number;
  status: EOrderRecordStatus | null;
  storeId: number | null;
}

export default OrderRecordsFiltersModel;