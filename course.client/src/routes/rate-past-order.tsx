import api from '#/api';
import Star from '#/components/assets/star';
import OrderRecordInfoModel from '#/models/server/orderRecordInfoModel';
import OrderRecordRatingPutModel from '#/models/server/orderRecordRatingPutModel';
import { useMutation } from '@tanstack/react-query';
import { createFileRoute, useRouter, useSearch } from '@tanstack/react-router';
import axios from 'axios';
import { FormEventHandler, useState } from 'react';

interface RatePastOrderParams {
  rating: number;
  orderRecord: OrderRecordInfoModel;
  inventoryRecord?: {
    inventoryRecordId: number;
    image: string | undefined;
    size: string | undefined;
  };
}

export const Route = createFileRoute('/rate-past-order')({
  component: RouteComponent,
  validateSearch: (search: Record<string, unknown>): RatePastOrderParams => {
    return {
      rating: Number(search?.rating ?? '1'),
      orderRecord: search.orderRecord as OrderRecordInfoModel,
      inventoryRecord: search.inventoryRecord as RatePastOrderParams['inventoryRecord'],
    };
  },
});

function RouteComponent() {
  const router = useRouter();
  const searchParams = useSearch({ from: '/rate-past-order' });
  const record = searchParams.orderRecord;
  const [rating, setRating] = useState(searchParams.rating);
  const [comment, setComment] = useState<string | null>(null);
  const [mutationError, setMutationError] = useState<string | null>(null);

  const ratingMutation = useMutation({
    mutationFn: async (data: OrderRecordRatingPutModel) => {
      return await axios.put(`/${api.client.order.rateRecord}`, data);
    },
    onSuccess() {
      router.navigate({
        to: '/past-order/$orderId',
        params: { orderId: record.orderId.toString() },
        reloadDocument: true,
      });
    },
    onError(error) {
      setMutationError('Ошибка: ' + error.message);
    },
  });

  const handleSubmit: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();

    await ratingMutation.mutateAsync({
      id: record.id,
      ratingValue: rating,
      ratingComment: comment ?? undefined,
    });
  };

  return (
    <div className="page gap-6">
      {searchParams.inventoryRecord?.image ? (
        <img
          key={record.inventoryRecordId}
          className={'rounded-md h-64 object-contain mx-auto'}
          src={`data:image/*;base64,${searchParams.inventoryRecord.image}`}
        />
      ) : (
        <></>
      )}
      <div className="flex flex-row gap-6 mx-auto py-4 px-8 bg-slate-700 rounded-full">
        <div className="flex flex-row-reverse w-fit order-rating-star-reset-container">
          {Array.from({ length: 5 }, (_, i) => (
            <button key={i} onClick={() => setRating(5 - i)} className="order-rating-star">
              <Star className={`size-6 ${rating >= 5 - i ? 'fill-slate-100' : ''}`} />
            </button>
          ))}
        </div>
        {rating} / 5
      </div>
      <form className="flex flex-col gap-6 flex-1" onSubmit={handleSubmit}>
        <textarea
          placeholder="Расскажите о своих впечатлениях..."
          className="flex-[0.5]"
          value={comment ?? ''}
          onChange={(e) => setComment(e.target.value === '' ? null : e.target.value)}
        />
        {mutationError && <p className="text-red-600">{mutationError}</p>}
        <button type="submit" className="btn ml-auto">
          Сохранить
        </button>
      </form>
    </div>
  );
}
