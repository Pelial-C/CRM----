import { apiGet } from './http'
import type { FoundationEnums } from '../types/foundation'

export const foundationApi = {
  getEnums: () => apiGet<FoundationEnums>('/foundation/enums'),
}
