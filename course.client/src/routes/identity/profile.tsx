import { useQuery, useQueryClient } from '@tanstack/react-query';
import { createFileRoute, Link } from '@tanstack/react-router';
import UserInfo from '#/models/server/requests/userInfo';
import api from '#/api';
import axios from 'axios';
import Orders from '#/components/orders';
import { authenticate } from '#/utils/http';

export const Route = createFileRoute('/identity/profile')({
  component: Profile,
  beforeLoad: authenticate,
});

function Profile() {
  const queryClient = useQueryClient();
  const query = useQuery<UserInfo>(
    {
      queryKey: ['user-info'],
      queryFn: async () => {
        const res = await axios.get(api.public.identity.userInfo);
        return res.data;
      },
    },
    queryClient,
  );

  return (
    <div className="page gap-4">
      <h2>Здравствуйте, {query.data?.info?.name}</h2>
      {query.data?.info?.isRegisteredSeller ? (<></>) : (<div>
        <Link from="/identity/profile" to="/identity/become-seller" className='link'>Стать продавцом</Link>
      </div>)}
      <div className="flex-1">
        <Orders />
      </div>
    </div>
  );
}
