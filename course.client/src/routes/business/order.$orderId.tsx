import { authenticateSeller } from '#/utils/http'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/business/order/$orderId')({
  component: ProcessOrder,
  beforeLoad: authenticateSeller,
})

function ProcessOrder() {
  return <div>Hello "/business/order/$orderId"!</div>
}
