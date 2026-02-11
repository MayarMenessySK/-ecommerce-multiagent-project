# QA Tester Agent - Skill Definition

## Agent Identity
**Role**: QA Tester / Quality Assurance Engineer  
**Responsibility**: Testing strategy, test case execution, automation, quality gates  
**Tech Stack**: xUnit, Playwright, Postman/REST Client, K6 (load testing)

---

## 🎯 BEFORE YOU START: Create GitHub Issue

```bash
gh issue create \
  --title "[QA] Write Tests and Validate System" \
  --body "## Agent: QA Tester

## Tasks
- [ ] Write unit tests (xUnit)
- [ ] Write integration tests
- [ ] Write E2E tests (Playwright)
- [ ] Validate test cases from PM
- [ ] Create test fixtures
- [ ] Generate coverage report

## Deliverables
- Unit tests for backend
- Integration tests for API
- E2E tests for frontend
- Test coverage report

## Dependencies
- Depends on: #3 (Backend), #4 (Frontend)

## Acceptance Criteria
- [ ] 80%+ code coverage
- [ ] All critical paths tested
- [ ] All PM test cases validated" \
  --label "agent-task,testing,in-progress"
```

📖 **See GITHUB-WORKFLOW.md for details**

---

## Core Competencies

### 1. Testing Strategy
- **Test Planning**: Comprehensive test plans for all features
- **Test Case Design**: Functional, integration, E2E, performance, security tests
- **Bug Tracking**: Detailed bug reports with reproduction steps
- **Test Coverage**: Ensure high coverage across all layers
- **Regression Testing**: Automated regression suite

### 2. Testing Levels

#### Unit Testing (xUnit)
- **Backend**: Service layer, repository logic
- **Frontend**: Component logic, utility functions
- **Coverage Target**: 80%+ code coverage
- **Mocking**: Moq for dependencies
- **Assertions**: FluentAssertions for readable tests

#### Integration Testing
- **API Testing**: Full request/response cycles
- **Database Testing**: Verify data persistence
- **External Services**: Mock Stripe, email services
- **Test Database**: Isolated test environment

#### End-to-End Testing (Playwright)
- **User Flows**: Complete user journeys
- **Cross-Browser**: Chrome, Firefox, Edge, Safari
- **Mobile Testing**: Responsive design validation
- **Visual Regression**: Screenshot comparison

#### Performance Testing
- **Load Testing**: K6 or JMeter
- **API Performance**: Response time benchmarks
- **Database Performance**: Query optimization
- **Frontend Performance**: Lighthouse audits

#### Security Testing
- **Authentication**: JWT token validation
- **Authorization**: Role-based access control
- **SQL Injection**: Parameterized query verification
- **XSS Protection**: Input sanitization tests
- **CSRF**: Token validation

## Technology Stack

### Testing Tools
```xml
<!-- Backend Testing -->
<ItemGroup>
  <PackageReference Include="xUnit" Version="2.6.0" />
  <PackageReference Include="xUnit.runner.visualstudio" Version="2.5.0" />
  <PackageReference Include="Moq" Version="4.20.0" />
  <PackageReference Include="FluentAssertions" Version="6.12.0" />
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.6.0" />
</ItemGroup>
```

```json
// Frontend Testing
{
  "devDependencies": {
    "vitest": "^1.0.0",
    "@testing-library/react": "^14.1.0",
    "@testing-library/jest-dom": "^6.1.0",
    "@testing-library/user-event": "^14.5.0",
    "playwright": "^1.40.0",
    "@playwright/test": "^1.40.0",
    "msw": "^2.0.0"
  }
}
```

## Test Structure

```
tests/
├── backend/
│   ├── Unit/
│   │   ├── ECommerce.Business.Tests/
│   │   │   └── Services/
│   │   │       ├── AreaServiceTests.cs
│   │   │       ├── ProductServiceTests.cs
│   │   │       └── OrderServiceTests.cs
│   │   └── ECommerce.Core.Tests/
│   │       └── Repositories/
│   └── Integration/
│       └── ECommerce.API.Tests/
│           ├── Controllers/
│           │   ├── ProductsControllerTests.cs
│           │   ├── OrdersControllerTests.cs
│           │   └── AuthControllerTests.cs
│           └── Scenarios/
│               ├── FullCheckoutScenarioTests.cs
│               └── UserJourneyTests.cs
│
├── frontend/
│   ├── unit/
│   │   ├── components/
│   │   │   ├── ProductCard.test.tsx
│   │   │   ├── CartItem.test.tsx
│   │   │   └── CheckoutForm.test.tsx
│   │   ├── hooks/
│   │   │   └── useAuth.test.ts
│   │   └── utils/
│   │       └── validation.test.ts
│   └── e2e/
│       ├── auth.spec.ts
│       ├── product-browsing.spec.ts
│       ├── cart.spec.ts
│       ├── checkout.spec.ts
│       └── admin.spec.ts
│
├── performance/
│   ├── load-test.js
│   └── stress-test.js
│
└── security/
    └── security-tests.md
```

## Backend Unit Testing Examples

### Service Layer Test
```csharp
using Xunit;
using Moq;
using FluentAssertions;
using ECommerce.Core.Features.Product;
using ECommerce.Data.Entities;

namespace ECommerce.Business.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IStringLocalizer<ProductService>> _mockLocalizer;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLocalizer = new Mock<IStringLocalizer<ProductService>>();
        _service = new ProductService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLocalizer.Object,
            Mock.Of<ILogger<ProductService>>());
    }

    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsSuccess()
    {
        // Arrange
        var productId = 1;
        var product = new ProductEntity { Id = productId, Name = "Test Product", Price = 29.99m };
        var productOutput = new ProductOutput { Id = productId, Name = "Test Product", Price = 29.99m };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockMapper.Setup(m => m.Map<ProductOutput>(product))
            .Returns(productOutput);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Product");
        result.Data.Price.Should().Be(29.99m);
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ReturnsFailure()
    {
        // Arrange
        var productId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((ProductEntity?)null);
        
        var localizedString = new LocalizedString("ProductNotFound", "Product not found");
        _mockLocalizer.Setup(l => l["ProductNotFound"]).Returns(localizedString);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Product not found");
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesProduct()
    {
        // Arrange
        var input = new CreateProductInput
        {
            Name = "New Product",
            Description = "Product description",
            Price = 49.99m,
            StockQuantity = 100
        };

        var product = new ProductEntity { Id = 1, Name = input.Name, Price = input.Price };
        var productOutput = new ProductOutput { Id = 1, Name = input.Name, Price = input.Price };

        _mockMapper.Setup(m => m.Map<ProductEntity>(input))
            .Returns(product);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<ProductEntity>()))
            .ReturnsAsync(product);
        _mockMapper.Setup(m => m.Map<ProductOutput>(product))
            .Returns(productOutput);

        // Act
        var result = await _service.CreateAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("New Product");
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<ProductEntity>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_InvalidName_ReturnsFailure(string invalidName)
    {
        // Arrange
        var input = new CreateProductInput { Name = invalidName };
        var localizedString = new LocalizedString("NameRequired", "Product name is required");
        _mockLocalizer.Setup(l => l["NameRequired"]).Returns(localizedString);

        // Act
        var result = await _service.CreateAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("required");
    }
}
```

## Backend Integration Testing

### API Integration Test with Test Containers
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using System.Net.Http.Json;

namespace ECommerce.API.Tests.Controllers;

public class ProductsControllerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public ProductsControllerTests()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("ecommerce_test")
            .WithUsername("postgres")
            .WithPassword("test123")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
                    });
                });
            });

        _client = _factory.CreateClient();

        // Run migrations
        await RunMigrationsAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductListOutput>>();
        products.Should().NotBeNull();
        products!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_ProductExists_ReturnsProduct()
    {
        // Arrange
        var productId = await CreateTestProductAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var product = await response.Content.ReadFromJsonAsync<ProductOutput>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task Create_ValidProduct_ReturnsCreated()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var input = new CreateProductInput
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 50
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var product = await response.Content.ReadFromJsonAsync<ProductOutput>();
        product.Should().NotBeNull();
        product!.Name.Should().Be("Test Product");
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _client?.Dispose();
        await _factory?.DisposeAsync();
    }

    private async Task RunMigrationsAsync()
    {
        // Trigger migration endpoint or run migrations programmatically
    }

    private async Task<int> CreateTestProductAsync()
    {
        // Create and return product ID
        return 1;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        // Login as admin and return JWT token
        return "test-token";
    }
}
```

## Frontend Unit Testing (Vitest + React Testing Library)

```typescript
// ProductCard.test.tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ProductCard } from './ProductCard';

describe('ProductCard', () => {
  const mockProduct = {
    id: '1',
    name: 'Test Product',
    description: 'Test description',
    price: 29.99,
    imageUrl: '/test.jpg',
    stockQuantity: 10
  };

  const renderWithClient = (component: React.ReactElement) => {
    const queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } }
    });
    return render(
      <QueryClientProvider client={queryClient}>
        {component}
      </QueryClientProvider>
    );
  };

  it('renders product information correctly', () => {
    renderWithClient(<ProductCard product={mockProduct} />);

    expect(screen.getByText('Test Product')).toBeInTheDocument();
    expect(screen.getByText('$29.99')).toBeInTheDocument();
    expect(screen.getByText('Test description')).toBeInTheDocument();
  });

  it('displays out of stock when quantity is zero', () => {
    const outOfStockProduct = { ...mockProduct, stockQuantity: 0 };
    renderWithClient(<ProductCard product={outOfStockProduct} />);

    expect(screen.getByText('Out of Stock')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /add to cart/i })).toBeDisabled();
  });

  it('handles add to cart click', async () => {
    const onAddToCart = vi.fn();
    renderWithClient(
      <ProductCard product={mockProduct} onAddToCart={onAddToCart} />
    );

    const addButton = screen.getByRole('button', { name: /add to cart/i });
    fireEvent.click(addButton);

    await waitFor(() => {
      expect(onAddToCart).toHaveBeenCalledWith(mockProduct.id);
    });
  });

  it('displays loading state during add to cart', async () => {
    renderWithClient(<ProductCard product={mockProduct} />);

    const addButton = screen.getByRole('button', { name: /add to cart/i });
    fireEvent.click(addButton);

    expect(await screen.findByText('Adding...')).toBeInTheDocument();
  });
});
```

## E2E Testing with Playwright

```typescript
// e2e/checkout.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Checkout Flow', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login');
    await page.fill('[name="email"]', 'test@example.com');
    await page.fill('[name="password"]', 'Password123!');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL('/');
  });

  test('complete purchase flow successfully', async ({ page }) => {
    // Browse products
    await page.goto('/products');
    await expect(page.locator('h1')).toHaveText('Products');

    // Add product to cart
    await page.click('[data-testid="product-card-1"] button:has-text("Add to Cart")');
    await expect(page.locator('[data-testid="cart-count"]')).toHaveText('1');

    // View cart
    await page.click('[data-testid="cart-icon"]');
    await expect(page).toHaveURL('/cart');
    await expect(page.locator('[data-testid="cart-item"]')).toHaveCount(1);

    // Proceed to checkout
    await page.click('button:has-text("Proceed to Checkout")');
    await expect(page).toHaveURL('/checkout');

    // Fill shipping information
    await page.fill('[name="firstName"]', 'John');
    await page.fill('[name="lastName"]', 'Doe');
    await page.fill('[name="address1"]', '123 Main St');
    await page.fill('[name="city"]', 'New York');
    await page.fill('[name="state"]', 'NY');
    await page.fill('[name="postalCode"]', '10001');
    await page.fill('[name="country"]', 'USA');
    await page.fill('[name="phone"]', '555-1234');

    // Fill payment information (Stripe test mode)
    const stripeFrame = page.frameLocator('iframe[name*="stripe"]');
    await stripeFrame.locator('[name="cardnumber"]').fill('4242424242424242');
    await stripeFrame.locator('[name="exp-date"]').fill('12/25');
    await stripeFrame.locator('[name="cvc"]').fill('123');

    // Place order
    await page.click('button:has-text("Place Order")');

    // Verify success
    await expect(page).toHaveURL(/\/orders\/[0-9]+/, { timeout: 10000 });
    await expect(page.locator('h1')).toHaveText('Order Confirmed');
    await expect(page.locator('[data-testid="order-number"]')).toBeVisible();
  });

  test('validates required fields in checkout', async ({ page }) => {
    // Add item to cart first
    await page.goto('/products');
    await page.click('[data-testid="product-card-1"] button');
    await page.click('[data-testid="cart-icon"]');
    await page.click('button:has-text("Proceed to Checkout")');

    // Try to submit without filling required fields
    await page.click('button:has-text("Place Order")');

    // Verify validation errors
    await expect(page.locator('text="First name is required"')).toBeVisible();
    await expect(page.locator('text="Address is required"')).toBeVisible();
  });

  test('handles payment failure gracefully', async ({ page }) => {
    // Setup test to use a card that will fail
    await page.goto('/checkout');
    
    // Fill form...
    const stripeFrame = page.frameLocator('iframe[name*="stripe"]');
    await stripeFrame.locator('[name="cardnumber"]').fill('4000000000000002'); // Stripe test card that fails

    await page.click('button:has-text("Place Order")');

    // Verify error handling
    await expect(page.locator('[data-testid="error-message"]')).toContainText('Payment failed');
  });
});

test.describe('Admin Panel', () => {
  test.beforeEach(async ({ page }) => {
    // Login as admin
    await page.goto('/login');
    await page.fill('[name="email"]', 'admin@example.com');
    await page.fill('[name="password"]', 'Admin123!');
    await page.click('button[type="submit"]');
  });

  test('admin can create new product', async ({ page }) => {
    await page.goto('/admin/products');
    await page.click('button:has-text("Add Product")');

    await page.fill('[name="name"]', 'New Test Product');
    await page.fill('[name="description"]', 'Product description');
    await page.fill('[name="price"]', '99.99');
    await page.fill('[name="stockQuantity"]', '50');
    
    await page.click('button:has-text("Save")');

    await expect(page.locator('text="Product created successfully"')).toBeVisible();
  });

  test('admin can view orders', async ({ page }) => {
    await page.goto('/admin/orders');

    await expect(page.locator('h1')).toHaveText('Orders');
    await expect(page.locator('[data-testid="order-row"]')).toHaveCount.greaterThan(0);
  });
});
```

## Test Cases Documentation

### Test Case Template
```markdown
# Test Case: TC-XXX - [Feature Name]

**Feature**: [Feature Area]  
**Priority**: High/Medium/Low  
**Type**: Functional/Integration/E2E/Performance/Security  
**Automated**: Yes/No  

**Preconditions**:
- List all preconditions
- Database state
- User authentication state

**Test Steps**:
1. Step 1
2. Step 2
3. Step 3

**Expected Results**:
- Expected outcome 1
- Expected outcome 2

**Test Data**:
- Input data required for test

**Postconditions**:
- State after test execution

**Notes**:
- Additional information
```

### Example Test Case
```markdown
# Test Case: TC-001 - User Registration

**Feature**: Authentication  
**Priority**: High  
**Type**: Functional  
**Automated**: Yes (E2E)  

**Preconditions**:
- Application is running
- Database is accessible
- Email address is not already registered

**Test Steps**:
1. Navigate to /register
2. Enter email: newuser@example.com
3. Enter password: SecurePass123!
4. Enter first name: John
5. Enter last name: Doe
6. Click "Register" button

**Expected Results**:
- User is created in database with role 'Customer'
- HTTP 201 status code returned
- JWT token is provided in response
- User is redirected to home page
- Success message displayed

**Test Data**:
- Email: newuser@example.com
- Password: SecurePass123!
- First Name: John
- Last Name: Doe

**Postconditions**:
- User record exists in database
- User can login with credentials
- Email verification email sent (if applicable)

**Notes**:
- Password must meet strength requirements
- Email must be unique
```

## Bug Report Template

```markdown
# Bug Report: BUG-XXX

**Title**: [Brief description]  
**Severity**: Critical/High/Medium/Low  
**Priority**: P0/P1/P2/P3  
**Status**: Open/In Progress/Fixed/Closed  

**Environment**:
- OS: Windows 11 / macOS / Linux
- Browser: Chrome 120
- Frontend Version: v1.2.3
- Backend Version: v1.2.3
- Database: PostgreSQL 16

**Description**:
Clear description of the bug

**Steps to Reproduce**:
1. Step 1
2. Step 2
3. Step 3

**Expected Behavior**:
What should happen

**Actual Behavior**:
What actually happens

**Screenshots/Videos**:
[Attach if applicable]

**Logs/Error Messages**:
```
Error stack trace or logs
```

**Additional Context**:
Any additional information
```

## Performance Testing (K6)

```javascript
// load-test.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '1m', target: 10 },  // Ramp up to 10 users
    { duration: '3m', target: 10 },  // Stay at 10 users
    { duration: '1m', target: 50 },  // Ramp up to 50 users
    { duration: '3m', target: 50 },  // Stay at 50 users
    { duration: '1m', target: 0 },   // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
    http_req_failed: ['rate<0.01'],   // Error rate should be less than 1%
  },
};

export default function () {
  // Test product listing
  let res = http.get('http://localhost:5000/api/v1/products');
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });

  sleep(1);

  // Test product detail
  res = http.get('http://localhost:5000/api/v1/products/1');
  check(res, {
    'status is 200': (r) => r.status === 200,
  });

  sleep(1);
}
```

## Continuous Testing

### GitHub Actions Integration
```yaml
# .github/workflows/tests.yml
name: Run Tests

on: [push, pull_request]

jobs:
  backend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Run Unit Tests
        run: dotnet test tests/backend/Unit/ --logger trx
      - name: Run Integration Tests
        run: dotnet test tests/backend/Integration/ --logger trx
      
  frontend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '20'
      - name: Install dependencies
        run: cd frontend && npm install
      - name: Run Unit Tests
        run: cd frontend && npm run test:unit
      - name: Run E2E Tests
        run: cd frontend && npx playwright install && npm run test:e2e
```

## Quality Gates

### Before Merge
- [ ] All unit tests pass (80%+ coverage)
- [ ] All integration tests pass
- [ ] Critical E2E tests pass
- [ ] No new security vulnerabilities
- [ ] Performance benchmarks met
- [ ] Code review approved

### Before Release
- [ ] Full E2E test suite passes
- [ ] Load testing completed
- [ ] Security audit passed
- [ ] Browser compatibility tested
- [ ] Accessibility audit (WCAG AA)
- [ ] Database migrations tested

## Git Workflow
- **Branch Naming**: `test/[feature-name]` or `fix/[bug-name]`
- **Test Files**: Commit test files with feature code
- **Bug Reports**: Create GitHub issues with bug template

## Deliverables
- [ ] Unit test suite for backend (80%+ coverage)
- [ ] Unit test suite for frontend (80%+ coverage)
- [ ] Integration test suite for API
- [ ] E2E test suite for critical user flows
- [ ] Performance test scripts
- [ ] Security test checklist
- [ ] Test documentation
- [ ] Bug reports with reproduction steps
- [ ] Test automation in CI/CD

## Success Criteria
- All tests automated and passing
- 80%+ code coverage across projects
- Zero critical bugs at release
- Performance targets met (< 200ms API, < 2.5s page load)
- Security vulnerabilities addressed
- Test execution integrated in CI/CD
- Comprehensive test documentation
