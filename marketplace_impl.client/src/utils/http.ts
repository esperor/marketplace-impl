import axios from 'axios';
import UserInfo from '../models/server/requests/userInfo';
import api from '../api';
import { redirect } from '@tanstack/react-router';

export const replaceRouteParams = (
  endpoint: string,
  params: { [key: string]: string | number },
) => {
  let url = endpoint;
  for (const key in params) {
    url = url.replace(`{${key}}`, params[key].toString());
  }
  return url;
};

const fetchUserData = async (): Promise<UserInfo> => {
  return axios.get(api.public.identity.userInfo).then((response) => response.data);
};

const authenticateInternal = (data: UserInfo, location: unknown) => {
  if (!data || !data.isSignedIn)
    throw redirect({
      to: '/identity/login',
      search: {
        returnUrl: (location as Location).href,
      },
    });
};

export const authenticate = async ({ location }: { location: unknown }) => {
  authenticateInternal(await fetchUserData(), location);
};

export const authenticateSeller = async ({ location }: { location: unknown }) => {
  const data = await fetchUserData();

  authenticateInternal(data, location);

  if (!data.info?.isRegisteredSeller)
    throw redirect({
      to: '/',
    });
};
