export interface InventoryRecordBase {
  id: number;
  quantity: number;
  size?: string;
  variation: string;
  price: number;
  image?: string;
  rating?: number;
}
export default interface InventoryRecordServer extends InventoryRecordBase {
  propertiesJson?: string;
}
