import { useQuery, useQueryClient } from '@tanstack/react-query';
import { addToCart, readCart, removeFromCart } from '../utils/cart';

function ProductCounter({
  recordId,
  productId,
  quantity,
  onChange,
}: {
  recordId: number;
  productId: number;
  quantity: number;
  onChange?: (recordId: number, newQuantity: number) => void;
}) {
  const queryClient = useQueryClient();
  const cartQuery = useQuery({
    queryKey: ['cart-storage'],
    queryFn: () => readCart(),
  });
  const cart = cartQuery.data;

  const addProductToCart = (recordId: number, productId: number) => {
    const recordQuantityInCart = cart!.find((item) => item?.recordId == recordId)?.quantity;
    if (recordQuantityInCart) removeFromCart(recordId);
    const newQuantity = recordQuantityInCart ? recordQuantityInCart + 1 : 1;
    addToCart({
      productId: productId,
      recordId: recordId,
      quantity: newQuantity,
    });
    if (onChange) onChange(recordId, newQuantity);
    queryClient.invalidateQueries({ queryKey: ['cart-storage'] });
  };

  const subtractProductFromCart = (recordId: number, productId: number) => {
    const recordQuantityInCart = cart!.find((item) => item?.recordId == recordId)?.quantity;
    if (recordQuantityInCart) {
      removeFromCart(recordId);
      const newQuantity = recordQuantityInCart - 1;
      if (recordQuantityInCart > 1)
        addToCart({
          productId: productId,
          recordId: recordId,
          quantity: newQuantity,
        });
      if (onChange) onChange(recordId, newQuantity);
    }
    queryClient.invalidateQueries({ queryKey: ['cart-storage'] });
  };

  if (!cart || cartQuery.isPending) return <p>Загрузка...</p>;

  const recordInCart = cart.find((item) => item?.recordId == recordId);

  return (
    <div className="w-fit p-[2px] flex-nowrap flex flex-row items-center bg-slate-800 rounded-full">
      {recordInCart && (
        <button
          type="button"
          className="w-8 h-8 bg-slate-700 rounded-full active:scale-90 relative"
          onClick={() => subtractProductFromCart(recordId, productId)}
        >
          <div className="w-4 h-[2px] bg-slate-100 absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"></div>
        </button>
      )}
      <p className="min-w-10 px-3 text-center text-lg">{recordInCart?.quantity ?? 0}</p>
      <button
        type="button"
        className="w-8 h-8 bg-slate-700 rounded-full active:scale-90 relative disabled:opacity-0 disabled:cursor-default"
        onClick={() => addProductToCart(recordId, productId)}
        disabled={(recordInCart?.quantity ?? 0) >= quantity}
      >
        <div className="w-4 h-[2px] bg-slate-100 absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"></div>
        <div className="w-4 h-[2px] bg-slate-100 absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 rotate-90"></div>
      </button>
    </div>
  );
}

export default ProductCounter;
