import api from '#/api';
import OrderRecordsFiltersModel from '#/models/orderRecordsFiltersModel';
import EOrderRecordStatus from '#/models/orderStatus';
import OrderRecordSellerServer from '#/models/server/orderRecordSellerServer';
import constant from '#/utils/constants';
import { useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useCallback, useEffect, useState } from 'react';
import useInfiniteQueryReduced from './useInfiniteQueryReduced';

const useOrders = (searchParams?: {
  limit?: number;
  status?: EOrderRecordStatus;
  storeId?: number;
}) => {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<OrderRecordsFiltersModel>({
    limit: constant.defaultLimit,
    status: searchParams?.status ?? null,
    storeId: searchParams?.storeId ?? null,
  });

  useEffect(() => {
    if (!searchParams) return;
    if (searchParams.limit) setFilters(prev => ({ ...prev, limit: searchParams.limit as number }));
    if (searchParams.status) setFilters(prev => ({ ...prev, status: searchParams.status ?? null }));
    if (searchParams.storeId) setFilters(prev => ({ ...prev, storeId: searchParams.storeId as number }));

    queryClient.invalidateQueries({ queryKey: ['seller-orders'] });
  }, [searchParams, queryClient]);

  const fetchOrders = useCallback(async ({ pageParam }: { pageParam: unknown }) => {
    let url = `/${api.business.order.getAll}?offset=${(pageParam as number) * filters.limit}&limit=${filters.limit}`;
    if (filters.status !== null) url += `&status=${filters.status}`;
    if (filters.storeId) url += `&storeId=${filters.storeId}`;

    return (await axios.get(url)).data;
  }, [filters]);

  const queryReduced = useInfiniteQueryReduced<OrderRecordSellerServer>({
    queryFn: async (props) => await fetchOrders(props),
    queryKey: ['seller-orders'],
    limit: filters.limit,
  });

  useEffect(() => {
    const search: Record<string, string> = {
      limit: filters.limit.toString(),
    };

    if (filters.status != null) search.status = filters.status.toString();
    if (filters.storeId != null) search.storeId = filters.storeId.toString();

    history.replaceState({}, '', `?${new URLSearchParams(search)}`);
  }, [filters]);

  return {
    filters,
    setFilters,
    ...queryReduced,
  };
};

export default useOrders;
