import api from '#/api'
import OrderInfo from '#/models/server/requests/orderInfo'
import { replaceRouteParams } from '#/utils/http'
import { createFileRoute } from '@tanstack/react-router'
import axios from 'axios'

export const Route = createFileRoute('/past-order/$orderId')({
  component: RouteComponent,
  loader: ({ params }) =>
    axios.get(
      replaceRouteParams(`/${api.client.order.get}`, { id: params.orderId }),
    ),
  wrapInSuspense: true,
})

function RouteComponent() {
  const orderRequest = Route.useLoaderData();
  const order = orderRequest.data as OrderInfo;

  return <div>{order.id}</div>
}
