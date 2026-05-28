import { createFileRoute } from '@tanstack/react-router';
import axios from 'axios';
import api from '../api';
import ProductAggregatedModel from '../models/server/productAggregatedModel';
import randomStock from '../utils/randomStock';
import ProductCounter from '../components/productCounter';
import { useEffect, useMemo, useState } from 'react';
import { replaceRouteParams } from '../utils/http';
import Star from '#/components/assets/star';
import ProductReviewModel from '#/models/server/productReviewModel';
import useInfiniteQueryReduced from '#/hooks/useInfiniteQueryReduced';
import { hashStringToColour } from '#/utils/hashColor';
import Profile from '#/components/assets/profile';

export const Route = createFileRoute('/product/$recordId')({
  component: Product,
  loader: ({ params }) =>
    axios.get(replaceRouteParams(`/${api.public.product.get}`, { id: params.recordId })),
  wrapInSuspense: true,
});

function Product() {
  const [chosenRecordId, setChosenRecordId] = useState<number>();
  const [chosenVariation, setChosenVariation] = useState<string>();
  const productRequest = Route.useLoaderData();

  const product = productRequest.data as ProductAggregatedModel;
  const isClothes = typeof product.records?.at(0)?.size === 'string';

  const reviewsQuery = useInfiniteQueryReduced({
    queryKey: ['record-reviews', chosenRecordId],
    queryFn: async (): Promise<ProductReviewModel[]> => {
      const response = await axios.get(
        replaceRouteParams(`/${api.public.product.record.getReviews}`, { id: chosenRecordId! }),
      );
      return response.data.map((item: ProductReviewModel) => ({
        ...item,
        ratingDate: new Date(item.ratingDate),
      }));
    },
    limit: 10,
    enabled: !!chosenRecordId,
  });

  const productPresent: boolean =
    (product.records &&
      product.records.length > 0 &&
      product.records.reduce(
        // check if any record is in stock
        (prev, curr) => prev || curr.quantity > 0,
        false,
      )) ||
    false;

  const productVariations = useMemo(() => {
    const productVariationsSet = new Set<string>();
    product.records?.forEach((record) => productVariationsSet.add(record.variation));

    return Array.from(productVariationsSet);
  }, [product.records]);

  useEffect(() => {
    if (!!productVariations && productVariations.length > 0) {
      setChosenVariation(productVariations[0]);
      if (!isClothes)
        setChosenRecordId(product.records?.find((r) => r.variation === productVariations[0])?.id);
    }
  }, [productVariations, product.records, isClothes]);

  const variationRecords = useMemo(() => {
    if (!isClothes) return null;

    return product.records?.filter((record) => record.variation === chosenVariation);
  }, [isClothes, product.records, chosenVariation]);

  useEffect(() => {
    if (!!variationRecords && variationRecords.length > 0) {
      setChosenRecordId(variationRecords[0].id);

    }
  }, [variationRecords]);

  const record = product.records?.find((record) => record.id === chosenRecordId);
  const recordProperties: Record<string, string> = JSON.parse(record?.propertiesJson ?? '{}');

  const rating = record?.rating;

  return (
    <div className="page bg-slate-900">
      <div
        className={`flex flex-row flex-wrap bg-slate-900 content-baseline px-8 relative gap-8 ${
          !productPresent ? 'bg-gray-700' : ''
        }`}
        key={product.id}
      >
        <div
          className={`flex-1 basis-96 max-h-[40dvh] z-[0] items-center flex justify-center ${productPresent ? '' : 'opacity-50'}`}
        >
          <img
            className="rounded-lg object-contain max-w-full max-h-full"
            src={
              record?.image ? `data:image/*;base64,${record.image}` : `/stock/${randomStock()}.jpg`
            }
            alt={product.title}
          />
        </div>
        <div className="flex flex-col h-[25%] flex-1 basis-96 pb-4 gap-2">
          <h3 className="flex flex-row flex-wrap items-center justify-between">
            <p
              title={product.title}
              className="overflow-hidden text-2xl font-medium overflow-ellipsis text-nowrap"
            >
              {product.title}
            </p>
            {product.records?.length ?? 0 > 0 ? null : <p className="text-xl ml-auto">Нет в наличии</p>}
            <div className="flex flex-row-reverse w-fit ml-auto">
              {!!rating &&
                Array.from({ length: 5 }, (_, i) => (
                  <Star key={i} className={`size-6 ${rating >= 5 - i ? 'fill-slate-100' : ''}`} />
                ))}
            </div>
            <p className="ml-2">{rating?.toFixed(2)}</p>
          </h3>
          <h4 className="z-[1] relative flex-row flex">
            <p
              title={product.storeName}
              className="text-slate-400 max-w-[50%] overflow-hidden overflow-ellipsis text-nowrap"
            >
              {product.storeName}
            </p>
          </h4>
          <div className="flex flex-row gap-4 justify-between mt-4">
            <div className="flex flex-col gap-2">
              <div className="flex flex-row flex-wrap gap-2">
                {productVariations.length > 0
                  ? productVariations.map((variation) => (
                      <button
                        key={variation}
                        type="button"
                        className={`rounded-md border-solid border-[1px] border-gray-700 w-fit py-1 px-3 ${
                          chosenVariation === variation ? 'bg-slate-700' : ''
                        }`}
                        onClick={() => {
                          setChosenVariation(variation);
                          if (!isClothes) {
                            setChosenRecordId(
                              product.records?.find((record) => record.variation === variation)?.id,
                            );
                          }
                        }}
                      >
                        {variation}
                      </button>
                    ))
                  : null}
              </div>
              {isClothes && variationRecords && variationRecords.length > 0 ? (
                <div className="flex flex-row flex-wrap gap-2">
                  {variationRecords.map((record) => (
                    <button
                      key={record.id}
                      type="button"
                      className={`rounded-md border-solid border-[1px] border-gray-700 w-fit py-1 px-3 ${
                        chosenRecordId === record.id ? 'bg-slate-700' : ''
                      }`}
                      onClick={() => setChosenRecordId(record.id)}
                    >
                      {record.size}
                    </button>
                  ))}
                </div>
              ) : null}
            </div>
            {!!record &&
              (record.quantity > 0 ? (
                <div key={record.id} className="flex flex-row gap-4 items-center">
                  <ProductCounter
                    recordId={record.id}
                    productId={product.id}
                    quantity={record.quantity}
                  />
                  <p className="text-xl font-medium">{`${record.price} руб.`}</p>
                </div>
              ) : (
                <p className="flex items-center">Нет в наличии</p>
              ))}
          </div>
          <p className="mt-4">{product.description}</p>
          {recordProperties && Object.keys(recordProperties).length > 0 ? (
            <div className="flex flex-col gap-2 mt-4">
              Характеристики:
              <ul className="list-disc ps-6">
                {Object.entries(recordProperties).map(([key, value]) => (
                  <li key={key}>
                    {key}: {value}
                  </li>
                ))}
              </ul>
            </div>
          ) : null}
        </div>
      </div>
      {reviewsQuery.data?.pages && (
        <div id="reviews-container" className="flex flex-col gap-4 m-6">
          <h4 className="px-8 py-2 text-xl font-semibold border border-transparent">Отзывы</h4>
          {reviewsQuery.data.pages.at(0)?.length === 0 && <p className='px-8 text-slate-200'><i>К сожалению, здесь пока пусто</i></p>}
          {reviewsQuery.data.pages.map((page) =>
            page.map((review, i) => (
              <div
                key={i}
                className="h-fit w-full border border-slate-400 px-8 py-4 rounded-lg flex flex-col gap-2"
              >
                <div className="flex flex-row gap-4">
                  <div
                    className="rounded-full w-12 h-12"
                    style={{ backgroundColor: hashStringToColour(JSON.stringify(review)) }}
                  >
                    <Profile className="size-[3.2rem] translate-x-[-0.1rem] translate-y-[-0.1rem]" />
                  </div>
                  <div className="flex flex-col gap-4">
                    <div className="flex flex-col">
                      <div className="flex flex-row gap-4 items-center">
                        <h5 className="font-semibold">Анонимный пользователь</h5>
                        <p className="text-slate-300">
                          {review.ratingDate.toLocaleDateString('ru')}
                        </p>
                      </div>
                      <div className="flex flex-row-reverse w-fit">
                        {Array.from({ length: 5 }, (_, i) => (
                          <Star
                            key={i}
                            className={`size-6 ${review.ratingValue >= 5 - i ? 'fill-slate-100' : ''}`}
                          />
                        ))}
                      </div>
                    </div>
                    {review.ratingComment && <p>{review.ratingComment}</p>}
                  </div>
                </div>
              </div>
            )),
          )}
          <reviewsQuery.LoadMoreBtn />
        </div>
      )}
    </div>
  );
}
