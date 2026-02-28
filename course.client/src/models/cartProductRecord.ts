import InventoryRecord from "./inventoryRecord";

export default interface CartProductRecord extends InventoryRecord {
  title: string;
  serverQuantity: number;
  productId: number;
  description: string;
}