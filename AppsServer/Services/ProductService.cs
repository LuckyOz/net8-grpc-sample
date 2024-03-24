
using Grpc.Core;
using AutoMapper;
using AppsServer.Protos;
using AppsServer.Models.Context;
using AppsServer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppsServer.Services
{
    public class ProductService(DataDbContext dataDbContext, IMapper mapper) : Product.ProductBase
    {
        public override async Task<ProductResponse> GetProductById (ProductRequest request, ServerCallContext context)
        {
            try
            {
                if (request is null || string.IsNullOrEmpty(request.Id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "id cannot be null"));

                var data = await GetProductById(request.Id);

                return data is null
                    ? throw new RpcException(new Status(StatusCode.NotFound, $"Data not found with id {request.Id}"))
                    : await Task.FromResult(data);

            } catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ProductAllResponse> GetAllProduct (ProductAllRequest request, ServerCallContext context)
        {
            try
            {
                var data = await GetAllProduct(request.Page, request.PageLimit);

                return data is null
                    ? throw new RpcException(new Status(StatusCode.NotFound, "Data not found"))
                    : await Task.FromResult(data);

            } catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ProductResponse> CreateProduct (ProductAddRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Name))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Code and Name cannot be null"));

                var dataCheck = 
                    await dataDbContext.MasterProducts.FirstOrDefaultAsync(q => q.Code == request.Code);

                if (dataCheck != null)
                    throw new RpcException(new Status(StatusCode.AlreadyExists, $"Code {request.Code} already exists"));

                var data = await CreateProduct(request);

                return data is null
                    ? throw new RpcException(new Status(StatusCode.Unknown, "Failed add product !!"))
                    : await Task.FromResult(data);

            } catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ProductResponse> UpdateProduct (ProductEditRequest request, ServerCallContext context)
        {
            try
            {
                if (request is null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.Name))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "id, code, and name cannot be null"));

                var data = await GetProductById(request.Id) 
                    ?? throw new RpcException(new Status(StatusCode.NotFound, $"Data not found with id {request.Id}"));

                var resutl = await UpdateProduct(request);

                return resutl is null
                    ? throw new RpcException(new Status(StatusCode.Unknown, "Failed update product !!"))
                    : await Task.FromResult(resutl);

            } catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ProducMessageResponse> DeleteProduct (ProductRequest request, ServerCallContext context)
        {
            try
            {
                if (request is null || string.IsNullOrEmpty(request.Id))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "id, code, and name cannot be null"));

                var data = await GetProductById(request.Id)
                    ?? throw new RpcException(new Status(StatusCode.NotFound, $"Data not found with id {request.Id}"));

                var resutl = await DeleteProduct(request);

                if (resutl.Message != $"Data with id {request.Id} has been deleted")
                    throw new RpcException(new Status(StatusCode.Unknown, "Failed delete product !!"));

                return resutl;

            } catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private async Task<ProductResponse> GetProductById(string id)
        {
            return (mapper.Map<ProductResponse>(
                await dataDbContext.MasterProducts.FirstOrDefaultAsync(q => q.Id.ToString() == id && q.ActiveFlag)));
        }

        private async Task<ProductAllResponse> GetAllProduct(int? page, int? page_limit)
        {
            page ??= 1;
            page_limit ??= 10;

            page = page == 0 ? 1 : page;
            page_limit = page_limit == 0 ? 10 : page_limit;

            var total_page = Math.Ceiling(await dataDbContext.MasterProducts
                .Where(q => q.ActiveFlag)
                .CountAsync() / (decimal)page_limit);

            var data = await dataDbContext.MasterProducts
                .Where(q => q.ActiveFlag)
                .OrderBy(q => q.Code)
                .Skip((page.Value - 1) * page_limit.Value)
                .Take(page_limit.Value)
                .ToListAsync();

            var dataMap = mapper.Map<List<ProductResponse>>(data);

            return new ProductAllResponse
            {
                Products = { dataMap },
                Page = page.Value,
                PageLimit = page_limit.Value,
                PageTotal = (int)total_page
            };
        }

        private async Task<ProductResponse> CreateProduct(ProductAddRequest request)
        {
            var dataMap = mapper.Map<MasterProduct>(request);

            dataDbContext.MasterProducts.Add(dataMap);

            await dataDbContext.SaveChangesAsync();

            return await GetProductById(dataMap.Id.ToString());
        }

        private async Task<ProductResponse> UpdateProduct(ProductEditRequest request)
        {
            var data = 
                await dataDbContext.MasterProducts.FirstOrDefaultAsync(q => q.Id.ToString() == request.Id);

            data!.Code = request.Code;
            data!.Name = request.Name;

            await dataDbContext.SaveChangesAsync();

            var dataNew = await GetProductById(request.Id); 

            return dataNew;
        }

        private async Task<ProducMessageResponse> DeleteProduct(ProductRequest request)
        {
            var data = 
                await dataDbContext.MasterProducts.FirstOrDefaultAsync(q => q.Id.ToString() == request.Id);

            data!.ActiveFlag = false;

            await dataDbContext.SaveChangesAsync();

            return new ProducMessageResponse
            {
                Message = $"Data with id {request.Id} has been deleted"
            };
        }
    }
}
