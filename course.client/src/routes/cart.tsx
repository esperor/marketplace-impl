import { createFileRoute } from '@tanstack/react-router';
import { useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import * as queries from '#/utils/queries';
import ProductCounter from '#/components/productCounter';
import randomStock from '#/utils/randomStock';
import { clearCart, removeFromCart } from '#/utils/cart';
import { useMemo, useState } from 'react';
import Trash from '#/components/assets/trash';

export const Route = createFileRoute('/cart')({
  component: Cart,
});

function Cart() {
  const queryClient = useQueryClient();
  const query = useSuspenseQuery(
    {
      queryKey: ['cart'],
      queryFn: queries.cart,
    },
    queryClient,
  );
  const [selectedItems, setSelectedItems] = useState(new Set(query.data.map((item) => item.id)));
  const { totalCost, totalQuantity } = useMemo(() => {
    if (query.data.length <= 0) return { totalCost: 0, totalQuantity: 0 };
    return query.data
      .map((item) =>
        item && [...selectedItems].includes(item.id)
          ? { quantity: item.quantity, cost: item.price * item.quantity }
          : { quantity: 0, cost: 0 },
      )
      .reduce(
        (prev, curr) => ({
          totalCost: prev.totalCost + curr.cost,
          totalQuantity: prev.totalQuantity + curr.quantity,
        }),
        { totalCost: 0, totalQuantity: 0 },
      );
  }, [query.data, selectedItems]);

  function handleCounterChange() {
    queryClient.invalidateQueries({ queryKey: ['cart'] });
  }

  function handleClearCart() {
    clearCart();
    queryClient.invalidateQueries({ queryKey: ['cart'] });
  }

  function handleRemove(recordId: number) {
    removeFromCart(recordId);
    queryClient.invalidateQueries({ queryKey: ['cart'] });
  }

  function handleCheckboxChange(e: React.ChangeEvent<HTMLInputElement>, recordId: number) {
    setSelectedItems((prev) => {
      if (e.target.checked) {
        return new Set([...prev, recordId]);
      } else {
        return new Set([...prev].filter((item) => item !== recordId));
      }
    });
  }

  if (query.isPending) return <p>Загрузка...</p>;
  if (query.isError) return <p>Произошла ошибка: {query.error.message}</p>;
  if (query.data?.length == 0 || query.data == null) return <p>Ваша корзина пуста</p>;

  return (
    <div className="flex flex-row gap-8 h-full py-4">
      <div className="flex flex-col gap-2 flex-1">
        <div className="flex flex-row gap-2 items-center">
          <h2 className="text-xl font-bold">Ваша корзина:</h2>
          <button
            type="button"
            className="ml-auto btn"
            disabled={query.data?.length == 0 || query.data == null}
            onClick={handleClearCart}
          >
            Очистить
          </button>
        </div>
        <div className="w-full overflow-y-scroll flex-1 flex flex-col gap-4">
          {query.data &&
            query.data
              .sort((cartRecord1, cartRecord2) =>
                // TODO: check if server sent an inventory record
                cartRecord1?.id > cartRecord2?.id ? 1 : -1,
              )
              .map(
                (cartRecord) =>
                  cartRecord && (
                    <div className="flex flex-row gap-2">
                      <input
                        type="checkbox"
                        className="w-7 h-7 bg-slate-700 outline-none flex-shrink-0"
                        checked={[...selectedItems].includes(cartRecord.id)}
                        onChange={(e) => handleCheckboxChange(e, cartRecord.id)}
                      />
                      <div
                        className="rounded-md shadow-md border-none relative bg-slate-900 flex flex-1 h-fit flex-row gap-4"
                        key={cartRecord?.id}
                      >
                        <img
                          className="w-40 aspect-square rounded-lg object-cover hover:object-contain"
                          src={
                            cartRecord.image
                              ? `data:image/*;base64,${cartRecord.image}`
                              : `/stock/${randomStock()}.jpg`
                          }
                        ></img>
                        <div className="flex flex-col w-full">
                          <div className="flex flex-row items-center my-2 mx-2 gap-4 justify-between">
                            <h3 className="text-left py-2 w-full overflow-clip text-xl font-semibold">
                              {cartRecord.title}
                            </h3>
                            <button
                              type="button"
                              className="active:scale-90 scale-100 p-2"
                              title="Удалить"
                              onClick={() => handleRemove(cartRecord.id)}
                            >
                              <Trash className="size-6" />
                            </button>
                          </div>
                          <div className="p-2 flex flex-row flex-wrap gap-2 justify-between flex-1">
                            <div className="self-start flex flex-row gap-1 items-center">
                              {!!cartRecord.size && (
                                <>
                                  <div className="bg-slate-700 rounded-md border-solid border-[1px] border-gray-700 w-fit py-1 px-3">
                                    {cartRecord.size}
                                  </div>
                                  <div className="w-2 h-2 bg-slate-300 rounded-full"></div>
                                </>
                              )}
                              <div className="bg-slate-700 rounded-md border-solid border-[1px] border-gray-700 w-fit py-1 px-3">
                                {cartRecord.variation}
                              </div>
                            </div>
                            <div className="flex flex-col gap-2 self-start flex-1 items-end">
                              <div className="flex flex-row w-full gap-2 items-center">
                                <p className="text-lg text-slate-50 mx-auto text-nowrap">
                                  {cartRecord.price * cartRecord.quantity} руб.
                                </p>
                                <ProductCounter
                                  recordId={cartRecord.id}
                                  productId={cartRecord.productId}
                                  quantity={cartRecord.serverQuantity}
                                  onChange={handleCounterChange}
                                />
                              </div>
                              {cartRecord.quantity > 1 ? (
                                <p className="text-slate-200">{cartRecord.price} руб. за ед.</p>
                              ) : (
                                <></>
                              )}
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  ),
              )}
        </div>
      </div>
      <div className="flex flex-col gap-4 basis-60">
        <div className="flex flex-col gap-2 bg-slate-900 rounded-md p-4">
          <div className="flex flex-row gap-2 justify-between">
            Товаров:<span>{totalQuantity} шт.</span>
          </div>
          <div className="flex flex-row gap-2 justify-between">
            Сумма:<span>{totalCost} руб.</span>
          </div>
        </div>
        <div className="flex flex-col gap-1">
          <Link
            from="/cart"
            to="/order"
            search={{ ids: [...selectedItems] }}
            className="btn w-full text-center"
            disabled={totalQuantity === 0}
          >
            Оформить заказ
          </Link>
          <p className="text-slate-300">Нужно будет указать способ оплаты и адрес доставки</p>
        </div>
      </div>
    </div>
  );
}

export default Cart;
