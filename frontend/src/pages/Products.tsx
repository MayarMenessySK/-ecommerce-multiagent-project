import { useState } from 'react';
import { useProducts } from '../api/queries/useProducts';
import { ProductFilters } from '../types/product.types';
import ProductCard from '../components/products/ProductCard';
import Loading from '../components/common/Loading';

const Products: React.FC = () => {
  const [filters, setFilters] = useState<ProductFilters>({
    page: 1,
    pageSize: 12,
  });
  
  const { data, isLoading } = useProducts(filters);
  
  const handlePageChange = (page: number) => {
    setFilters({ ...filters, page });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };
  
  if (isLoading && !data) return <Loading fullScreen />;
  
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">All Products</h1>
      
      {data && data.items.length > 0 ? (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {data.items.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
          
          {/* Pagination */}
          {data.totalPages > 1 && (
            <div className="flex justify-center items-center gap-2 mt-12">
              <button
                onClick={() => handlePageChange(data.pageNumber - 1)}
                disabled={!data.hasPreviousPage}
                className="px-4 py-2 border rounded-lg disabled:opacity-50 hover:bg-gray-50"
              >
                Previous
              </button>
              
              <span className="px-4 py-2">
                Page {data.pageNumber} of {data.totalPages}
              </span>
              
              <button
                onClick={() => handlePageChange(data.pageNumber + 1)}
                disabled={!data.hasNextPage}
                className="px-4 py-2 border rounded-lg disabled:opacity-50 hover:bg-gray-50"
              >
                Next
              </button>
            </div>
          )}
        </>
      ) : (
        <p className="text-gray-600 text-center py-12">No products found.</p>
      )}
    </div>
  );
};

export default Products;
