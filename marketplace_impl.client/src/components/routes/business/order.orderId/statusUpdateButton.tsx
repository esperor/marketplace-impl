import api from '#/api';
import EOrderRecordStatus from '#/models/orderStatus';
import { replaceRouteParams } from '#/utils/http';
import { QueryClient, useMutation } from '@tanstack/react-query';
import { useParams } from '@tanstack/react-router';
import axios, { AxiosResponse, AxiosError } from 'axios';
import { useState } from 'react';

const StatusUpdateButton = ({
  selectedRecordId,
  selectedRecordStatus,
  queryClient,
}: {
  selectedRecordId: number;
  selectedRecordStatus: EOrderRecordStatus;
  queryClient: QueryClient;
}) => {
  const pathParams = useParams({ from: '/business/order/$orderId' });
  const [updateStatusError, setUpdateStatusError] = useState<string | null>(null);
  const updateStatus = useMutation<AxiosResponse, AxiosError<string>, EOrderRecordStatus>(
    {
      mutationFn: async (newStatus: EOrderRecordStatus) => {
        setUpdateStatusError(null);
        return await axios.put(
          `/${replaceRouteParams(api.business.order.record.updateStatus, { id: selectedRecordId })}`,
          { newStatus },
        );
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['seller-order', pathParams.orderId] });
      },
      onError: (error) => setUpdateStatusError(error.response!.data),
    },
    queryClient,
  );

  if (selectedRecordStatus === EOrderRecordStatus.Created)
    return (
      <div className="flex flex-col gap-1">
        <p className="text-yellow-400">Ожидает упаковки</p>
        <button
          type="button"
          className="btn"
          onClick={async () => await updateStatus.mutateAsync(EOrderRecordStatus.Packaged)}
        >
          Пометить как собранный
        </button>
        {updateStatusError && (
          <p className="text-red-500">
            При обновлении статуса произошла ошибка: {updateStatusError}
          </p>
        )}
      </div>
    );

  if (selectedRecordStatus === EOrderRecordStatus.Packaged)
    return (
      <div className="flex flex-col gap-1">
        <p className="text-green-500">Помечен как собранный</p>
        <button
          type="button"
          className="btn"
          onClick={async () => await updateStatus.mutateAsync(EOrderRecordStatus.Created)}
        >
          Снять метку
        </button>
        {updateStatusError && (
          <p className="text-red-500">
            При обновлении статуса произошла ошибка: {updateStatusError}
          </p>
        )}
      </div>
    );
};

export default StatusUpdateButton;
