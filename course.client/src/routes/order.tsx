import { createFileRoute } from '@tanstack/react-router';
import { authenticate } from '../utils/http';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import api from '../api';
import { useState } from 'react';
import OrderPostModel from '../models/server/requests/orderPostModel';
import { clearCart, readCart } from '../utils/cart';
import { InputMask, Mask } from '@react-input/mask';

export const Route = createFileRoute('/order')({
  component: Order,
  beforeLoad: authenticate,
  validateSearch: (search: Record<string, unknown>): { ids: number[] } => {
    return {
      ids: (search?.ids as number[]) || [],
    };
  },
});

function Order() {
  const queryClient = useQueryClient();
  const cartQuery = useQuery({
    queryKey: ['cart-storage'],
    queryFn: () => readCart(),
  });
  const [address, setAddress] = useState<{
    city: string;
    street: string;
    building: string;
    entrance: string;
    floor: string;
    flat: string;
  }>({
    city: '',
    street: '',
    building: '',
    entrance: '',
    floor: '',
    flat: '',
  });
  const [paymentMethod, setPaymentMethod] = useState<'card' | 'cash'>('card');
  const cardNumberMask = '____ ____ ____ ____';
  const cardExpiryMask = '__/__';
  const cardCvvMask = '___';
  const commonReplacement = { _: /\d/ };
  const cardNumberMaskHelper = new Mask({
    mask: cardNumberMask,
    replacement: commonReplacement,
  });
  const cardCvvMaskHelper = new Mask({
    mask: cardCvvMask,
    replacement: commonReplacement,
  });
  const [cardInfo, setCardInfo] = useState({
    number: '',
    expiry: '',
    cvv: '',
  }); 
  const [error, setError] = useState<string | null>(null);
  const [hasOrdered, setHasOrdered] = useState(false);
  const postOrder = useMutation(
    {
      mutationFn: async (formData: OrderPostModel) => {
        return await axios.post(api.client.order.create, formData);
      },
      onSuccess() {
        clearCart();
        setHasOrdered(true);
        queryClient.invalidateQueries({ queryKey: ['cart'] });
        queryClient.invalidateQueries({ queryKey: ['cart-storage'] });
        queryClient.invalidateQueries({ queryKey: ['user-orders'] });
        queryClient.invalidateQueries({ queryKey: ['products'] });
      },
    },
    queryClient,
  );

  const handleSubmit: React.FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    await queryClient.invalidateQueries({ queryKey: ['cart-storage'] });
    const cart = await queryClient.fetchQuery({
      queryKey: ['cart-storage'],
      queryFn: () => readCart(),
    });
    if (cart.length === 0) {
      setError('Корзина пуста');
      return;
    }
    const records = cart.reduce(
      (acc, item) => {
        acc[item.recordId] = item.quantity;
        return acc;
      },
      {} as { [key: number]: number },
    );
    const form = {
      address: JSON.stringify(address),
      // Placeholder: doesn't actually send any info
      // cardInfo: paymentMethod === 'card' ? {
      //   number: cardNumberMaskHelper.unformat(cardInfo.number),
      //   expiry: cardInfo.expiry,
      //   cvv: cardCvvMaskHelper.unformat(cardInfo.cvv),
      // } : undefined,
      orderedRecords: records,
    };
    postOrder.mutate(form);
  };

  const addressLabels = {
    city: 'Населённый пункт',
    street: 'Улица',
    building: 'Номер здания',
    entrance: 'Подъезд',
    floor: 'Этаж',
    flat: 'Квартира/офис',
  };

  const addressPlaceholders = {
    city: 'Пермь',
    street: 'Ленина',
    building: '1А',
    entrance: '1',
    floor: '1',
    flat: '1',
  };

  return (
    <div className="page">
      <h1 className="text-xl font-bold">Заказать доставку</h1>
      <form className="flex flex-col gap-4 mt-6" onSubmit={handleSubmit}>
        <h3 className="text-lg font-semibold">Адрес</h3>
        <div className="flex flex-row flex-wrap gap-4">
          {Object.entries(address).map(([key]) => (
            <div key={key} className="flex flex-col gap-1">
              <label htmlFor={key}>{addressLabels[key as keyof typeof addressLabels]}</label>
              <input
                required
                type="text"
                id={key}
                placeholder={addressPlaceholders[key as keyof typeof addressPlaceholders]}
                onChange={(e) => setAddress({ ...address, [key]: e.target.value })}
              />
            </div>
          ))}
        </div>
        <h3 className="text-lg font-semibold">Способ оплаты</h3>
        <div className="flex flex-row gap-2 items-center">
          <input
            type="radio"
            className="w-5 h-5"
            id="card"
            name="payment-method"
            checked={paymentMethod === 'card'}
            onChange={() => setPaymentMethod('card')}
          />
          <label htmlFor="card">Картой онлайн</label>
        </div>
        <div className="flex flex-row gap-2 items-center">
          <input
            type="radio"
            className="w-5 h-5"
            id="cash"
            name="payment-method"
            checked={paymentMethod === 'cash'}
            onChange={() => setPaymentMethod('cash')}
          />
          <label htmlFor="cash">Наличными курьеру</label>
        </div>
        {paymentMethod === 'card' && (
          <div className="p-4 aspect-[86/54] w-3/12 min-w-64 gap-6 flex flex-col justify-between border-gray-400 border-solid border-2 bg-transparent rounded-lg">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className='w-[15%]'
              viewBox="0 0 30.6 20.8"
              fill="none"
              stroke="currentColor"
              strokeWidth={0.8}
            >
              <rect x="0.5" y="0.5" width="29.6" height="19.76" rx="3.6" />
              <line x1="19.4" y1="8.3" x2="30.1" y2="8.3" />
              <line x1="19.4" y1="12.4" x2="30.1" y2="12.4" />
              <line x1="0.6" y1="8.3" x2="11.2" y2="8.3" />
              <line x1="0.6" y1="12.4" x2="11.2" y2="12.4" />
              <path
                d="M17.4.6V6.3a4.7,4.7,0,0,1,2.9,4.3,4.9,4.9,0,0,1-2.9,4.3v5.6"
                transform="translate(-0.5 -0.2)"
              />
              <path
                d="M14.1.6V6.3a4.7,4.7,0,0,0-2.9,4.2,4.9,4.9,0,0,0,2.9,4.4v5.6"
                transform="translate(-0.5 -0.2)"
              />
            </svg>
            <div className="flex flex-col gap-6">
            <InputMask
              type="tel"
              placeholder="1234 5678 9012 3456"
              pattern="\d{4} \d{4} \d{4} \d{4}"
              mask={cardNumberMask}
              replacement={commonReplacement}
              required
              value={cardInfo.number}
              onChange={(e) => setCardInfo(prev => ({ ...prev, number: e.target.value}))}
              className="w-full bg-transparent outline-none border border-solid border-slate-400 px-2 py-1 text-slate-200"
            />
            <div className="flex flex-row justify-between gap-10">
              <InputMask
                type="tel"
                placeholder="01/24"
                required
                pattern="(0[1-9]|1[0-2])\/\d{2}"
                mask={cardExpiryMask}
                replacement={commonReplacement}
                value={cardInfo.expiry}
                onChange={(e) => setCardInfo(prev => ({ ...prev, expiry: e.target.value}))}
                className="w-20 bg-transparent outline-none border border-solid border-slate-400 px-2 py-1 text-slate-200"
              />
              <InputMask
                type="text"
                placeholder="CVV"
                pattern='\d{3}'
                mask={cardCvvMask}
                replacement={commonReplacement}
                required
                value={cardInfo.cvv}
                onChange={(e) => setCardInfo(prev => ({ ...prev, cvv: e.target.value}))}
                className="bg-transparent outline-none border border-solid border-slate-400 px-2 py-1 text-slate-200 w-16"
              />
            </div>
          </div></div>
        )}
        <div className="flex flex-row gap-4 items-center">
          <button
            className="btn"
            type="submit"
            disabled={hasOrdered || cartQuery.data?.length === 0}
          >
            {hasOrdered ? 'Заказ оформлен' : 'Оформить и оплатить'}
          </button>
          {error && <p className="text-red-600">{error}</p>}
        </div>
      </form>
    </div>
  );
}
