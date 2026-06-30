import { useQuery, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { customerApi } from '../../api/customerApi'
import ConfirmButton from '../../components/ConfirmButton'
import ErrorMessage from '../../components/ErrorMessage'
import Loading from '../../components/Loading'
import PageHeader from '../../components/PageHeader'
import { formatDate } from '../../utils/date'

const pageSize = 10

function CustomerListPage() {
  const queryClient = useQueryClient()
  const search = new URLSearchParams(window.location.search)
  const pageIndex = Number(search.get('page') ?? 1)
  const keyword = search.get('keyword') ?? ''
  const industry = search.get('industry') ?? ''
  const includeDeleted = search.get('includeDeleted') === 'true'

  const { data = [], isLoading, error } = useQuery({
    queryKey: ['customers', keyword, industry, includeDeleted],
    queryFn: () => customerApi.getList({ keyword, industry, includeDeleted }),
  })

  const totalPages = Math.max(1, Math.ceil(data.length / pageSize))
  const pageItems = data.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)

  const submitSearch = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const form = new FormData(event.currentTarget)
    const params = new URLSearchParams()
    const nextKeyword = String(form.get('keyword') ?? '').trim()
    const nextIndustry = String(form.get('industry') ?? '').trim()
    if (nextKeyword) params.set('keyword', nextKeyword)
    if (nextIndustry) params.set('industry', nextIndustry)
    if (form.get('includeDeleted') === 'on') params.set('includeDeleted', 'true')
    window.location.search = params.toString()
  }

  return (
    <>
      <PageHeader
        title="客户管理"
        description="维护企业客户和联系人"
        actions={<Link className="btn btn-primary" to="/customers/create">新增客户</Link>}
      />
      <form className="page-band mb-3" onSubmit={submitSearch}>
        <div className="row g-3 align-items-end">
          <div className="col-md-4">
            <label className="form-label">企业名称 / 信用代码</label>
            <input name="keyword" className="form-control" defaultValue={keyword} />
          </div>
          <div className="col-md-3">
            <label className="form-label">行业</label>
            <input name="industry" className="form-control" defaultValue={industry} />
          </div>
          <div className="col-md-3">
            <div className="form-check mt-4">
              <input className="form-check-input" id="includeDeleted" name="includeDeleted" type="checkbox" defaultChecked={includeDeleted} />
              <label className="form-check-label" htmlFor="includeDeleted">显示已删除客户</label>
            </div>
          </div>
          <div className="col-md-2">
            <button className="btn btn-outline-primary w-100" type="submit">搜索</button>
          </div>
        </div>
      </form>
      <ErrorMessage message={error instanceof Error ? error.message : undefined} />
      {isLoading ? <Loading /> : null}
      <div className="page-band">
        <div className="table-responsive">
          <table className="table table-hover">
            <thead>
              <tr>
                <th>企业名称</th>
                <th>统一社会信用代码</th>
                <th>行业</th>
                <th>地址</th>
                <th>创建时间</th>
                <th>状态</th>
                <th>操作</th>
              </tr>
            </thead>
            <tbody>
              {pageItems.map((customer) => (
                <tr key={customer.id}>
                  <td>{customer.name}</td>
                  <td>{customer.creditCode ?? '-'}</td>
                  <td>{customer.industry ?? '-'}</td>
                  <td>{[customer.province, customer.city, customer.district, customer.detailAddress].filter(Boolean).join(' ') || '-'}</td>
                  <td>{formatDate(customer.creationTime)}</td>
                  <td>
                    <span className={`badge badge-fixed ${customer.isDeleted ? 'text-bg-secondary' : 'text-bg-success'}`}>
                      {customer.isDeleted ? '已删除' : '正常'}
                    </span>
                  </td>
                  <td>
                    <div className="btn-group btn-group-sm">
                      <Link className="btn btn-outline-primary" to={`/customers/${customer.id}`}>详情</Link>
                      <Link className="btn btn-outline-secondary" to={`/customers/${customer.id}/edit`}>编辑</Link>
                      <ConfirmButton
                        className="btn btn-outline-danger"
                        message="确认删除该客户？有关联合同客户将进行逻辑删除。"
                        disabled={customer.isDeleted}
                        onConfirm={async () => {
                          await customerApi.remove(customer.id)
                          await queryClient.invalidateQueries({ queryKey: ['customers'] })
                        }}
                      >
                        删除
                      </ConfirmButton>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="d-flex justify-content-between align-items-center">
          <span className="text-secondary">共 {data.length} 条</span>
          <div className="btn-group">
            <Link className={`btn btn-outline-secondary btn-sm ${pageIndex <= 1 ? 'disabled' : ''}`} to={`?page=${pageIndex - 1}`}>
              上一页
            </Link>
            <span className="btn btn-outline-secondary btn-sm disabled">{pageIndex} / {totalPages}</span>
            <Link className={`btn btn-outline-secondary btn-sm ${pageIndex >= totalPages ? 'disabled' : ''}`} to={`?page=${pageIndex + 1}`}>
              下一页
            </Link>
          </div>
        </div>
      </div>
    </>
  )
}

export default CustomerListPage
