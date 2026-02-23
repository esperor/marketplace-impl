import api from '#/api';
import { useQueryClient, useMutation } from '@tanstack/react-query';
import { createFileRoute, LinkProps } from '@tanstack/react-router';
import axios from 'axios';
import { useState } from 'react';

export const Route = createFileRoute('/identity/become-seller')({
  component: RouteComponent,
});

function RouteComponent() {
  const [form, setForm] = useState({
    email: null as string | null,
    contractConditionsAccepted: false,
  });
  const queryClient = useQueryClient();
  const register = useMutation({
    mutationFn: (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      return axios.post(`/${api.public.identity.becomeSeller}`, form);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user-info'] });
      const businessUrl: LinkProps['to'] = '/business';
      window.location.href = businessUrl;
    },
  });

  const isFormFilled = !!form.email && form.contractConditionsAccepted;

  return (
    <div>
      <form onSubmit={register.mutate} className="flex flex-col mx-auto w-fit gap-2 max-w-64">
        <label htmlFor="email">Email</label>
        <input
          type="text"
          id="email"
          className=""
          onChange={(e) => setForm({ ...form, email: e.target.value })}
        />
        <div className="flex flex-row gap-4">
          <input
            type="checkbox"
            id="conditions"
            onChange={(e) => setForm({ ...form, contractConditionsAccepted: e.target.checked })}
          />
          <label htmlFor="conditions">
            Согласен с{' '}
            <a className="link" href="/terms-and-conditions.html">
              Условиями использования
            </a>{' '}
            и{' '}
            <a className="link" href="/privacy-policy.html">
              Политикой конфиденциальности
            </a>
          </label>
        </div>
        <label className='text-slate-300'>Маркетплейс будет взимать комиссию 15% с ваших продаж</label>
        <button type="submit" className="mx-auto btn" disabled={!isFormFilled}>
          Продолжить
        </button>
        <label>{register.isError && register.error.message}</label>
      </form>
    </div>
  );
}
