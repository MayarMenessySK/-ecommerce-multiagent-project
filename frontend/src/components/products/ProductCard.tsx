import { Link } from 'react-router-dom';
import { ProductListItem } from '../../types/product.types';
import Button from '../common/Button';
import { useAddToCart } from '../../api/queries/useCart';

interface ProductCardProps {
  product: ProductListItem;
}

const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  const addToCart = useAddToCart();
  
  const handleAddToCart = () => {
    addToCart.mutate({
      productId: product.id,
      quantity: 1,
    });
  };
  
  const discountAmount = product.originalPrice 
    ? ((product.originalPrice - product.price) / product.originalPrice * 100).toFixed(0)
    : null;
  
  const isOutOfStock = product.stockQuantity === 0;
  
  return (
    <div className="group relative bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition-all duration-300">
      {/* Discount Badge */}
      {discountAmount && (
        <div className="absolute top-2 left-2 bg-red-500 text-white px-2 py-1 rounded-md text-sm font-bold z-10">
          -{discountAmount}%
        </div>
      )}
      
      {/* Out of Stock Badge */}
      {isOutOfStock && (
        <div className="absolute top-2 right-2 bg-gray-800 text-white px-2 py-1 rounded-md text-sm font-bold z-10">
          Out of Stock
        </div>
      )}
      
      {/* Product Image */}
      <Link to={`/products/${product.slug}`}>
        <div className="relative h-64 overflow-hidden">
          <img
            src={product.primaryImage || 'https://via.placeholder.com/400x400?text=No+Image'}
            alt={product.name}
            className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
            loading="lazy"
          />
        </div>
      </Link>
      
      {/* Product Details */}
      <div className="p-4">
        <Link to={`/products/${product.slug}`}>
          <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2 hover:text-blue-600 transition-colors">
            {product.name}
          </h3>
        </Link>
        
        <p className="text-sm text-gray-600 mb-3">{product.categoryName}</p>
        
        {/* Rating */}
        <div className="flex items-center gap-2 mb-3">
          <div className="flex items-center">
            {[...Array(5)].map((_, i) => (
              <svg
                key={i}
                className={`h-4 w-4 ${
                  i < Math.floor(product.averageRating)
                    ? 'text-yellow-400'
                    : 'text-gray-300'
                }`}
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
            ))}
          </div>
          <span className="text-sm text-gray-600">
            ({product.totalReviews})
          </span>
        </div>
        
        {/* Price */}
        <div className="flex items-center gap-2 mb-4">
          <span className="text-2xl font-bold text-blue-600">
            ${product.price.toFixed(2)}
          </span>
          {product.originalPrice && (
            <span className="text-lg text-gray-400 line-through">
              ${product.originalPrice.toFixed(2)}
            </span>
          )}
        </div>
        
        {/* Add to Cart Button */}
        <Button
          variant="primary"
          fullWidth
          onClick={handleAddToCart}
          disabled={isOutOfStock || addToCart.isPending}
          isLoading={addToCart.isPending}
        >
          {isOutOfStock ? 'Out of Stock' : 'Add to Cart'}
        </Button>
      </div>
    </div>
  );
};

export default ProductCard;
