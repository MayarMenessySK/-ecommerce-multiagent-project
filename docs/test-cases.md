# E-Commerce System - Test Cases

**Version:** 1.0  
**Date:** December 2024  
**Project:** Multi-Agent E-Commerce Platform  
**Test Environment:** Staging  
**Test Type:** Manual & Automated

---

## Table of Contents
1. [User Management Test Cases](#user-management-test-cases)
2. [Product Catalog Test Cases](#product-catalog-test-cases)
3. [Shopping Cart Test Cases](#shopping-cart-test-cases)
4. [Checkout and Orders Test Cases](#checkout-and-orders-test-cases)
5. [Admin Panel Test Cases](#admin-panel-test-cases)
6. [Security Test Cases](#security-test-cases)
7. [Performance Test Cases](#performance-test-cases)
8. [Localization Test Cases](#localization-test-cases)

---

## User Management Test Cases

### TC-001: User Registration - Happy Path
**Requirement**: FR-001  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Application accessible
- Database connection available
- Email service configured
- Test email not already registered

**Test Data**:
- Email: `test.user@example.com`
- Password: `SecurePass123!`
- First Name: `John`
- Last Name: `Doe`
- Phone: `+1234567890`

**Test Steps**:
1. Navigate to registration page (`/register`)
2. Enter email address
3. Enter password
4. Enter password confirmation (same as password)
5. Enter first name
6. Enter last name
7. Enter phone number
8. Check "Accept Terms and Conditions"
9. Click "Register" button

**Expected Results**:
- Registration successful message displayed
- User created in database with Customer role
- Email verification sent to user
- User automatically logged in
- JWT token generated and stored
- Redirected to dashboard or home page
- HTTP 201 Created response

**Postconditions**:
- User exists in database
- User can login with credentials
- Verification email sent

**Test Status**: ✅ Pass / ❌ Fail / ⏸️ Blocked

---

### TC-002: User Registration - Duplicate Email
**Requirement**: FR-001  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Preconditions**:
- Email `existing@example.com` already registered

**Test Steps**:
1. Navigate to registration page
2. Enter existing email: `existing@example.com`
3. Enter valid password
4. Fill other required fields
5. Submit registration form

**Expected Results**:
- Error message: "Email already exists"
- HTTP 409 Conflict response
- User not created
- No email sent
- User remains on registration page

---

### TC-003: User Registration - Weak Password
**Requirement**: FR-001  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Data**:
- Weak passwords to test:
  - `12345678` (no uppercase, special char)
  - `password` (no number, uppercase, special char)
  - `Pass1!` (too short)
  - `PASSWORD123!` (no lowercase)

**Test Steps**:
1. For each weak password:
2. Navigate to registration page
3. Enter valid email
4. Enter weak password
5. Fill other fields
6. Submit form

**Expected Results**:
- Error message displayed: "Password must contain at least 8 characters, including uppercase, lowercase, number, and special character"
- Form submission prevented
- Inline validation error shown
- User remains on registration page

---

### TC-004: User Login - Valid Credentials
**Requirement**: FR-002  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User registered with email: `john.doe@example.com`
- User account active

**Test Steps**:
1. Navigate to login page (`/login`)
2. Enter email: `john.doe@example.com`
3. Enter correct password
4. Click "Login" button

**Expected Results**:
- Login successful
- JWT access token generated
- Refresh token generated
- Token expiry set correctly (15 min for access token)
- User redirected to intended page or dashboard
- User session active
- HTTP 200 OK response

---

### TC-005: User Login - Invalid Credentials
**Requirement**: FR-002  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Steps**:
1. Navigate to login page
2. Enter email: `john.doe@example.com`
3. Enter wrong password: `WrongPass123!`
4. Click "Login" button

**Expected Results**:
- Error message: "Invalid email or password"
- HTTP 401 Unauthorized response
- Failed attempt logged
- User remains on login page
- No token generated

---

### TC-006: User Login - Account Lockout
**Requirement**: FR-002  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Steps**:
1. Attempt login with wrong password 5 times
2. Verify account locked
3. Attempt login with correct password

**Expected Results**:
- After 5 failed attempts, account locked for 15 minutes
- Error message: "Account temporarily locked. Please try again in 15 minutes."
- HTTP 423 Locked response
- Correct credentials don't work during lockout period
- Account automatically unlocked after 15 minutes

---

### TC-007: User Profile - View Profile
**Requirement**: FR-003  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in

**Test Steps**:
1. Navigate to profile page (`/profile`)
2. View profile information

**Expected Results**:
- Display user details:
  - Email
  - First name, last name
  - Phone number
  - Profile picture
  - Account creation date
  - Last login date
- Saved addresses displayed
- Edit profile button visible

---

### TC-008: User Profile - Update Profile
**Requirement**: FR-003  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in

**Test Steps**:
1. Navigate to profile page
2. Click "Edit Profile"
3. Update first name to "Jane"
4. Update phone to "+9876543210"
5. Click "Save Changes"

**Expected Results**:
- Success message: "Profile updated successfully"
- Changes saved to database
- Updated information displayed
- HTTP 200 OK response

---

### TC-009: User Profile - Upload Profile Picture
**Requirement**: FR-003  
**Priority**: Low  
**Type**: Functional  
**Automation**: Partial

**Preconditions**:
- User logged in

**Test Data**:
- Valid image: JPG, 2MB
- Invalid image: PDF file
- Large image: JPG, 10MB

**Test Steps**:
1. Click "Upload Profile Picture"
2. Select valid image (JPG, 2MB)
3. Upload image
4. Verify upload successful
5. Try invalid file (PDF)
6. Try large file (10MB)

**Expected Results**:
- Valid image uploads successfully
- Image displayed in profile
- PDF rejected with error: "Only JPG/PNG allowed"
- 10MB file rejected: "Maximum file size is 5MB"
- Image stored in CDN/cloud storage

---

### TC-010: Password Reset - Request Reset
**Requirement**: FR-004  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User exists with email: `john.doe@example.com`

**Test Steps**:
1. Navigate to login page
2. Click "Forgot Password?"
3. Enter email: `john.doe@example.com`
4. Click "Send Reset Link"

**Expected Results**:
- Success message: "If the email exists, a reset link has been sent"
- Email sent with reset link
- Reset token generated (1-hour expiry)
- Token stored in database
- HTTP 200 OK response
- Same response for non-existent email (prevent enumeration)

---

### TC-011: Password Reset - Reset with Valid Token
**Requirement**: FR-004  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Reset link received in email
- Token not expired

**Test Steps**:
1. Click reset link in email
2. Enter new password: `NewSecure123!`
3. Confirm new password: `NewSecure123!`
4. Click "Reset Password"

**Expected Results**:
- Success message: "Password reset successful"
- Password updated in database
- Token invalidated (single use)
- All existing sessions terminated
- Confirmation email sent
- User can login with new password

---

### TC-012: Password Reset - Expired Token
**Requirement**: FR-004  
**Priority**: Medium  
**Type**: Negative  
**Automation**: Yes

**Preconditions**:
- Reset token expired (>1 hour old)

**Test Steps**:
1. Click expired reset link
2. Attempt to reset password

**Expected Results**:
- Error message: "Reset link expired. Please request a new one."
- HTTP 410 Gone response
- Password not changed
- Redirect to forgot password page

---

### TC-013: RBAC - Customer Access
**Requirement**: FR-005  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in with Customer role

**Test Steps**:
1. Attempt to access customer-only resources:
   - View own profile: `/profile`
   - View own orders: `/orders`
   - Add to cart: `/cart`
2. Attempt to access admin resources:
   - Admin dashboard: `/admin`
   - Product management: `/admin/products`

**Expected Results**:
- Customer resources accessible
- Admin resources return 403 Forbidden
- Error message: "You don't have permission"
- Redirected to access denied page

---

### TC-014: RBAC - Admin Access
**Requirement**: FR-005  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in with Admin role

**Test Steps**:
1. Access admin resources:
   - Dashboard: `/admin`
   - Products: `/admin/products`
   - Orders: `/admin/orders`
2. Attempt to access SuperAdmin resources:
   - User management: `/admin/users`

**Expected Results**:
- Admin resources accessible
- SuperAdmin resources return 403 Forbidden
- Can manage products and orders
- Cannot manage users

---

### TC-015: RBAC - SuperAdmin Access
**Requirement**: FR-005  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in with SuperAdmin role

**Test Steps**:
1. Access all resources:
   - Customer resources
   - Admin resources
   - SuperAdmin resources (user management)

**Expected Results**:
- Full access to all resources
- Can manage users, roles
- Can access all admin features

---

## Product Catalog Test Cases

### TC-016: Product Listing - Default View
**Requirement**: FR-006  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to products page (`/products`)
2. View product listing

**Expected Results**:
- Products displayed in grid view (default)
- 20 products per page
- Each product shows:
  - Image
  - Name
  - Price
  - Rating
  - Add to Cart button
- Pagination controls visible
- Sort by "Featured" (default)

---

### TC-017: Product Listing - Filtering
**Requirement**: FR-006  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to products page
2. Apply category filter: "Electronics"
3. Apply price range: $50 - $200
4. Apply rating filter: 4+ stars
5. Apply availability: "In Stock"

**Expected Results**:
- Products filtered according to criteria
- Product count updated
- Only matching products displayed
- Filter tags shown
- Can remove individual filters
- Can clear all filters

---

### TC-018: Product Listing - Sorting
**Requirement**: FR-006  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to products page
2. Sort by "Price: Low to High"
3. Verify order
4. Sort by "Price: High to Low"
5. Sort by "Best Rating"
6. Sort by "Newest"

**Expected Results**:
- Products sorted correctly each time
- Prices in ascending order (Low to High)
- Prices in descending order (High to Low)
- Highest rated products first (Best Rating)
- Newest products first by date

---

### TC-019: Product Search - Basic Search
**Requirement**: FR-007  
**Priority**: High  
**Type**: Functional  
**Automation': Yes

**Test Steps**:
1. Enter "wireless headphones" in search bar
2. Press Enter or click search

**Expected Results**:
- Search results page displayed
- Relevant products shown
- Search term highlighted in results
- Product count displayed
- Filters available on results page

---

### TC-020: Product Search - Autocomplete
**Requirement**: FR-007  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Type "wir" in search bar (minimum 3 chars)
2. Wait for suggestions

**Expected Results**:
- Dropdown with suggestions appears
- Suggestions include:
  - Product names
  - Categories
- Up to 10 suggestions shown
- Can click suggestion to search
- Can use keyboard to navigate (arrow keys)

---

### TC-021: Product Search - No Results
**Requirement**: FR-007  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Search for "xyzabc123" (non-existent)

**Expected Results**:
- "No results found" message
- Suggestions for alternative searches
- Popular products shown
- Search tips displayed

---

### TC-022: Product Details - View Details
**Requirement**: FR-008  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Click on product from listing
2. View product details page

**Expected Results**:
- Product details displayed:
  - Multiple images (zoomable)
  - Name, SKU
  - Price (current, original if discounted)
  - Stock status
  - Description (full)
  - Specifications
  - Variations (if applicable)
  - Reviews and rating
- Add to Cart button
- Add to Wishlist button
- Share buttons
- Related products section
- Breadcrumb navigation

---

### TC-023: Product Details - Image Gallery
**Requirement**: FR-008  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Partial

**Test Steps**:
1. View product with multiple images
2. Click on thumbnail images
3. Click main image to zoom
4. Use arrow keys to navigate

**Expected Results**:
- Clicking thumbnail updates main image
- Main image zoomable on click/hover
- Can navigate with arrow keys
- Image gallery responsive on mobile

---

### TC-024: Product Details - Variations
**Requirement**: FR-008  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View product with variations (e.g., color, size)
2. Select different color
3. Select different size
4. Check stock availability

**Expected Results**:
- Variation options displayed
- Can select color and size
- Price updates if variation affects price
- Stock status updates per variation
- Image updates per variation (if applicable)
- Cannot add to cart without selecting all required variations

---

### TC-025: Product Categories - Navigation
**Requirement**: FR-009  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View category menu
2. Navigate to "Electronics"
3. Navigate to subcategory "Audio"
4. Navigate to sub-subcategory "Headphones"

**Expected Results**:
- Category hierarchy displayed
- Breadcrumb navigation shows path
- Product count per category
- Category images displayed
- Category descriptions shown
- SEO-friendly URLs (e.g., `/electronics/audio/headphones`)

---

### TC-026: Product Categories - Category Page
**Requirement**: FR-009  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to category page
2. View category details and products

**Expected Results**:
- Category banner/image
- Category description
- Subcategories displayed (if any)
- Products in category listed
- Filtering and sorting available
- Product count shown

---

### TC-027: Product Categories - Empty Category
**Requirement**: FR-009  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to category with no products

**Expected Results**:
- "No products in this category" message
- Suggestions to browse other categories
- Subcategories still displayed

---

### TC-028: Product Reviews - Submit Review
**Requirement**: FR-010  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in
- User purchased the product
- User hasn't reviewed this product yet

**Test Steps**:
1. Navigate to purchased product
2. Click "Write a Review"
3. Select 5-star rating
4. Enter review title: "Excellent product!"
5. Enter review text (100 characters)
6. Upload image (optional)
7. Submit review

**Expected Results**:
- Review submitted successfully
- Message: "Review submitted for moderation"
- Review shows "Pending" status
- Email notification sent to admin
- Cannot submit duplicate review

---

### TC-029: Product Reviews - Edit Review
**Requirement**: FR-010  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User has existing review

**Test Steps**:
1. Navigate to product with user's review
2. Click "Edit Review"
3. Update rating to 4 stars
4. Update review text
5. Save changes

**Expected Results**:
- Review updated successfully
- Changes reflected immediately
- Updated timestamp shown
- Admin notified for re-moderation

---

### TC-030: Product Reviews - Review Restrictions
**Requirement**: FR-010  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Steps**:
1. Attempt to review without purchase
2. Attempt duplicate review

**Expected Results**:
- Error: "You can only review purchased products"
- Error: "You have already reviewed this product"
- Cannot submit review

---

## Shopping Cart Test Cases

### TC-031: Add to Cart - Single Item
**Requirement**: FR-011  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to product details
2. Select quantity: 1
3. Select variations (if applicable)
4. Click "Add to Cart"

**Expected Results**:
- Success message: "Added to cart"
- Cart icon shows item count
- Cart total updated
- Product added to cart with correct details
- Can view cart

---

### TC-032: Add to Cart - Stock Validation
**Requirement**: FR-011  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Preconditions**:
- Product has 5 items in stock

**Test Steps**:
1. Select quantity: 10
2. Attempt to add to cart

**Expected Results**:
- Error: "Only 5 items available in stock"
- Cannot add more than available
- Quantity input shows max value

---

### TC-033: Add to Cart - Out of Stock
**Requirement**: FR-011  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Preconditions**:
- Product out of stock (quantity: 0)

**Test Steps**:
1. View out-of-stock product
2. Attempt to add to cart

**Expected Results**:
- "Add to Cart" button disabled
- "Out of Stock" message displayed
- Option to "Notify When Available"

---

### TC-034: View Cart - Cart Summary
**Requirement**: FR-012  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Cart has 2 items

**Test Steps**:
1. Navigate to cart page (`/cart`)
2. View cart contents

**Expected Results**:
- All cart items listed with:
  - Product image
  - Name
  - Price
  - Quantity
  - Subtotal
- Cart summary shows:
  - Subtotal
  - Tax
  - Shipping
  - Total
- "Continue Shopping" button
- "Proceed to Checkout" button

---

### TC-035: Update Cart - Change Quantity
**Requirement**: FR-012  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to cart
2. Update quantity from 2 to 5
3. Click update

**Expected Results**:
- Quantity updated
- Subtotal recalculated
- Total updated
- Success message shown

---

### TC-036: Remove from Cart
**Requirement**: FR-012  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to cart
2. Click "Remove" on item
3. Confirm removal

**Expected Results**:
- Item removed from cart
- Cart total updated
- Item count decreased
- Success message: "Item removed"
- If cart empty, show "Cart is empty" message

---

### TC-037: Cart Persistence - Guest User
**Requirement**: FR-013  
**Priority': Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. As guest, add items to cart
2. Close browser
3. Reopen browser
4. Navigate to site

**Expected Results**:
- Cart items still present
- Stored in browser local storage
- Persists for 30 days
- Can continue shopping

---

### TC-038: Cart Persistence - Logged-in User
**Requirement**: FR-013  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Login and add items to cart
2. Logout
3. Login from different device

**Expected Results**:
- Cart synced across devices
- Items stored in database
- Cart accessible from any device
- Real-time sync

---

### TC-039: Cart Merge - Guest to Logged-in
**Requirement**: FR-013  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Guest cart has 2 items
- User account cart has 3 items

**Test Steps**:
1. As guest, add 2 items to cart
2. Login to existing account

**Expected Results**:
- Guest cart merged with user cart
- Total 5 items in cart
- No duplicates (quantities combined if same product)
- Guest cart cleared
- User cart updated

---

### TC-040: Discount Code - Valid Code
**Requirement**: FR-014  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Cart total: $200
- Discount code "SAVE20" active (20% off)

**Test Steps**:
1. Navigate to cart
2. Enter code: "SAVE20"
3. Click "Apply"

**Expected Results**:
- Success: "Discount applied"
- Discount: $40 (20% of $200)
- New total: $160 (plus tax/shipping)
- Discount shown in summary
- Code displayed in cart

---

### TC-041: Discount Code - Invalid Code
**Requirement**: FR-014  
**Priority**: Medium  
**Type**: Negative  
**Automation**: Yes

**Test Steps**:
1. Enter invalid code: "INVALID123"
2. Click "Apply"

**Expected Results**:
- Error: "Invalid discount code"
- No discount applied
- Total unchanged

---

### TC-042: Discount Code - Minimum Purchase
**Requirement**: FR-014  
**Priority**: Medium  
**Type**: Negative  
**Automation**: Yes

**Preconditions**:
- Cart total: $50
- Code "SAVE20" requires minimum $100 purchase

**Test Steps**:
1. Enter code: "SAVE20"
2. Attempt to apply

**Expected Results**:
- Error: "Minimum purchase of $100 required"
- Discount not applied
- Current total shown

---

## Checkout and Orders Test Cases

### TC-043: Checkout - Complete Checkout
**Requirement**: FR-015  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in
- Cart has items
- Stripe test mode enabled

**Test Steps**:
1. Click "Proceed to Checkout"
2. **Step 1: Shipping Address**
   - Select saved address or enter new
   - Click "Continue"
3. **Step 2: Shipping Method**
   - Select "Standard Shipping ($0)"
   - Click "Continue"
4. **Step 3: Payment**
   - Enter Stripe test card: 4242 4242 4242 4242
   - Expiry: 12/25, CVV: 123
   - Click "Pay Now"
5. **Step 4: Confirmation**
   - Review order
   - Accept terms
   - Click "Place Order"

**Expected Results**:
- All steps completed successfully
- Progress indicator shows current step
- Can go back to edit previous steps
- Order created
- Payment processed
- Order confirmation shown
- Cart cleared

---

### TC-044: Checkout - Guest Checkout
**Requirement**: FR-015  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Not logged in
- Cart has items

**Test Steps**:
1. Proceed to checkout
2. Select "Checkout as Guest"
3. Enter email address
4. Complete checkout process

**Expected Results**:
- Guest checkout allowed
- Email required
- Complete checkout without registration
- Order confirmation sent to email
- Option to create account after purchase

---

### TC-045: Checkout - Validation Errors
**Requirement**: FR-015  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Steps**:
1. Attempt checkout without address
2. Attempt checkout without payment method
3. Leave required fields empty

**Expected Results**:
- Cannot proceed without required fields
- Inline validation errors shown
- Helpful error messages
- Fields highlighted in red

---

### TC-046: Payment - Successful Payment
**Requirement**: FR-016  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Test Data**:
- Stripe test card: 4242 4242 4242 4242

**Test Steps**:
1. Enter payment details
2. Submit payment

**Expected Results**:
- Payment processed successfully
- Stripe payment intent created
- Transaction ID generated
- Payment confirmation
- No card details stored locally
- Card tokenized via Stripe

---

### TC-047: Payment - Failed Payment
**Requirement**: FR-016  
**Priority**: High  
**Type**: Negative  
**Automation**: Yes

**Test Data**:
- Stripe test card (decline): 4000 0000 0000 0002

**Test Steps**:
1. Enter declined test card
2. Attempt payment

**Expected Results**:
- Error: "Payment declined"
- Order not created
- User can retry with different card
- Helpful error message
- Order items still in cart

---

### TC-048: Payment - 3D Secure
**Requirement**: FR-016  
**Priority**: High  
**Type**: Functional  
**Automation**: Partial

**Test Data**:
- Stripe 3DS test card: 4000 0027 6000 3184

**Test Steps**:
1. Enter 3DS test card
2. Complete 3D Secure challenge

**Expected Results**:
- 3DS authentication modal appears
- Can complete authentication
- Payment processed after auth
- Secure payment flow

---

### TC-049: Order Confirmation
**Requirement**: FR-017  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Order successfully placed

**Test Steps**:
1. View order confirmation page

**Expected Results**:
- Order number displayed (e.g., ORD-2024-001234)
- Order summary shown
- Confirmation email sent
- Email contains:
  - Order number
  - Items
  - Total
  - Shipping address
  - Estimated delivery
- Download invoice button
- Link to order tracking

---

### TC-050: Order Confirmation - Email
**Requirement**: FR-017  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Check email after order placement

**Expected Results**:
- Confirmation email received within 5 minutes
- Email professionally formatted
- Contains all order details
- Link to view order online
- Customer support contact info

---

### TC-051: Order - Download Invoice
**Requirement**: FR-017  
**Priority**: Low  
**Type**: Functional  
**Automation**: Partial

**Test Steps**:
1. Navigate to order details
2. Click "Download Invoice"

**Expected Results**:
- PDF invoice downloads
- Invoice contains:
  - Company details
  - Invoice number
  - Order date
  - Items with prices
  - Subtotal, tax, shipping, total
  - Payment method
  - Billing/shipping addresses

---

### TC-052: Order History - View Orders
**Requirement**: FR-018  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User has placed orders

**Test Steps**:
1. Navigate to "My Orders"
2. View order history

**Expected Results**:
- All orders listed (newest first)
- Each order shows:
  - Order number
  - Date
  - Status
  - Total
  - Quick view button
- Pagination for many orders

---

### TC-053: Order History - Filter Orders
**Requirement**: FR-018  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Filter by status: "Delivered"
2. Filter by date range
3. Search by order number

**Expected Results**:
- Filters applied correctly
- Only matching orders shown
- Can combine multiple filters
- Can clear filters

---

### TC-054: Order History - Reorder
**Requirement**: FR-018  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View past order
2. Click "Reorder"

**Expected Results**:
- All items from order added to cart
- Cart quantities match order
- User directed to cart
- Can modify before checkout

---

### TC-055: Order Details - View Details
**Requirement**: FR-019  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Click on order from history
2. View order details

**Expected Results**:
- Order number and date
- Current status
- All items listed with images
- Shipping address
- Payment method (last 4 digits)
- Order timeline
- Tracking information (if shipped)
- Invoice download
- Cancel button (if eligible)

---

### TC-056: Order Details - Access Control
**Requirement**: FR-019  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. As User A, attempt to view User B's order

**Expected Results**:
- 403 Forbidden response
- Error message
- Cannot access other users' orders
- Redirected to access denied page

---

### TC-057: Order Details - Order Timeline
**Requirement**: FR-019  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View order details
2. Check order timeline

**Expected Results**:
- Timeline shows all status changes:
  - Pending
  - Processing
  - Shipped
  - Delivered
- Each status has timestamp
- Visual timeline display
- Current status highlighted

---

### TC-058: Order Tracking - Track Order
**Requirement**: FR-020  
**Priority**: Medium  
**Type**: Functional  
**Automation': Partial

**Preconditions**:
- Order status: Shipped
- Tracking number assigned

**Test Steps**:
1. View shipped order
2. Check tracking information

**Expected Results**:
- Shipping carrier displayed
- Tracking number shown
- Link to carrier tracking
- Estimated delivery date
- Current location (if available)

---

### TC-059: Order Tracking - Status Updates
**Requirement**: FR-020  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Admin updates order status to "Shipped"
2. Check customer receives notification

**Expected Results**:
- Email notification sent to customer
- Email contains tracking info
- Order status updated in account
- Timeline updated

---

### TC-060: Order Tracking - Delivery Confirmation
**Requirement**: FR-020  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Admin marks order as "Delivered"

**Expected Results**:
- Delivery email sent
- Actual delivery date recorded
- Request for review sent (after 3 days)
- Order marked complete

---

## Admin Panel Test Cases

### TC-061: Admin Dashboard - View Dashboard
**Requirement**: FR-021  
**Priority**: High  
**Type**: Functional  
**Automation': Yes

**Preconditions**:
- Logged in as Admin

**Test Steps**:
1. Navigate to `/admin`
2. View dashboard

**Expected Results**:
- Sales statistics displayed:
  - Today, week, month, year
  - Revenue charts
- Order count by status
- Top selling products
- Recent orders table
- Low stock alerts
- User registration stats
- Quick action buttons

---

### TC-062: Admin Dashboard - Sales Charts
**Requirement**: FR-021  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Partial

**Test Steps**:
1. View sales chart
2. Change date range

**Expected Results**:
- Chart displays correctly
- Data accurate
- Can toggle time periods
- Interactive chart (hover for details)

---

### TC-063: Admin Dashboard - Low Stock Alerts
**Requirement**: FR-021  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View low stock section

**Expected Results**:
- Products below threshold shown
- Stock quantity displayed
- Link to restock
- Sorted by criticality

---

### TC-064: Product Management - Create Product
**Requirement**: FR-022  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Logged in as Admin

**Test Steps**:
1. Navigate to `/admin/products`
2. Click "Add New Product"
3. Fill product form:
   - Name: "New Test Product"
   - SKU: "TEST-001"
   - Description: Full description
   - Price: 99.99
   - Category: Electronics
   - Stock: 50
4. Upload product images
5. Click "Save"

**Expected Results**:
- Product created successfully
- Product ID generated
- Product visible in listing
- Images uploaded to CDN
- Can view product on frontend

---

### TC-065: Product Management - Edit Product
**Requirement**: FR-022  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to product list
2. Click "Edit" on product
3. Update price to 89.99
4. Update stock to 75
5. Save changes

**Expected Results**:
- Changes saved
- Updates reflected immediately
- Audit log entry created
- Frontend shows updated info

---

### TC-066: Product Management - Delete Product
**Requirement**: FR-022  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Select product to delete
2. Click "Delete"
3. Confirm deletion

**Expected Results**:
- Product soft deleted
- Not visible on frontend
- Still in database (flagged as deleted)
- Can restore if needed
- Orders with product unaffected

---

### TC-067: Category Management - Create Category
**Requirement**: FR-023  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to `/admin/categories`
2. Click "Add Category"
3. Enter name: "New Category"
4. Select parent: "Electronics"
5. Upload category image
6. Save

**Expected Results**:
- Category created
- Appears in hierarchy
- Visible on frontend
- Products can be assigned

---

### TC-068: Category Management - Reorder Categories
**Requirement**: FR-023  
**Priority**: Low  
**Type**: Functional  
**Automation': Partial

**Test Steps**:
1. Drag and drop categories
2. Save new order

**Expected Results**:
- Order updated
- Changes reflected on frontend
- Display order saved

---

### TC-069: Category Management - Delete Category
**Requirement**: FR-023  
**Priority': Medium  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Category has products

**Test Steps**:
1. Attempt to delete category with products
2. Reassign products to different category
3. Delete category

**Expected Results**:
- Warning: "Category has products"
- Must reassign products first
- Products moved to new category
- Category deleted
- Frontend updated

---

### TC-070: Order Management - View All Orders
**Requirement**: FR-024  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Logged in as Admin

**Test Steps**:
1. Navigate to `/admin/orders`
2. View order list

**Expected Results**:
- All orders displayed
- Paginated list
- Search functionality
- Filters (status, date, customer)
- Export to CSV option

---

### TC-071: Order Management - Update Order Status
**Requirement**: FR-024  
**Priority': Critical  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Open order details
2. Update status to "Shipped"
3. Enter tracking number
4. Save

**Expected Results**:
- Status updated
- Customer notified via email
- Timeline updated
- Tracking number saved

---

### TC-072: Order Management - Refund Order
**Requirement**: FR-024  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Open order details
2. Click "Refund"
3. Select full or partial refund
4. Enter reason
5. Process refund

**Expected Results**:
- Refund processed via Stripe
- Status changed to "Refunded"
- Customer notified
- Amount returned to payment method
- Refund confirmation email sent

---

### TC-073: User Management - List Users
**Requirement**: FR-025  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Logged in as SuperAdmin

**Test Steps**:
1. Navigate to `/admin/users`
2. View user list

**Expected Results**:
- All users displayed
- User details shown:
  - Name, email
  - Role
  - Registration date
  - Status
- Search and filter options
- Pagination

---

### TC-074: User Management - Change User Role
**Requirement**: FR-025  
**Priority': Critical  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Logged in as SuperAdmin

**Test Steps**:
1. Select user (currently Customer)
2. Change role to Admin
3. Save

**Expected Results**:
- Role updated in database
- User now has Admin permissions
- Audit log entry created
- User notified of role change

---

### TC-075: User Management - Deactivate User
**Requirement**: FR-025  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Select active user
2. Click "Deactivate Account"
3. Confirm

**Expected Results**:
- User account deactivated
- User cannot login
- Existing sessions terminated
- Can reactivate later
- Orders preserved

---

### TC-076: Discount Management - Create Code
**Requirement**: FR-026  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to `/admin/coupons`
2. Click "Create Coupon"
3. Enter:
   - Code: "SUMMER20"
   - Type: Percentage
   - Value: 20%
   - Minimum: $50
   - Valid: 30 days
   - Usage limit: 100
4. Save

**Expected Results**:
- Coupon created
- Active immediately
- Customers can use code
- Usage tracked

---

### TC-077: Discount Management - Edit Code
**Requirement**: FR-026  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Edit existing coupon
2. Change value to 15%
3. Update expiry date
4. Save

**Expected Results**:
- Changes applied
- New value effective immediately
- Existing orders unaffected

---

### TC-078: Discount Management - Deactivate Code
**Requirement**: FR-026  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Select active coupon
2. Deactivate

**Expected Results**:
- Coupon deactivated
- Cannot be used
- Error shown if attempted
- Can reactivate later

---

### TC-079: Review Moderation - Approve Review
**Requirement**: FR-027  
**Priority': Medium  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- Pending reviews exist

**Test Steps**:
1. Navigate to `/admin/reviews`
2. View pending review
3. Click "Approve"

**Expected Results**:
- Review status: Approved
- Visible on product page
- Customer notified
- Product rating updated

---

### TC-080: Review Moderation - Reject Review
**Requirement**: FR-027  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View inappropriate review
2. Click "Reject"
3. Enter reason
4. Confirm

**Expected Results**:
- Review status: Rejected
- Not visible on product page
- Customer notified with reason
- Can be appealed

---

### TC-081: Review Moderation - Delete Spam
**Requirement**: FR-027  
**Priority**: Low  
**Type': Functional  
**Automation**: Yes

**Test Steps**:
1. Identify spam review
2. Delete review

**Expected Results**:
- Review permanently deleted
- Not counted in product ratings
- User flagged if repeated spam

---

### TC-082: Reports - Sales Report
**Requirement**: FR-028  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to Reports
2. Select "Sales Report"
3. Choose date range
4. Generate report

**Expected Results**:
- Report generated
- Shows:
  - Total sales
  - Order count
  - Average order value
  - Sales by product/category
- Export to CSV/PDF

---

### TC-083: Reports - Customer Report
**Requirement**: FR-028  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Generate Customer Report
2. View top customers

**Expected Results**:
- Customer lifetime value shown
- Purchase frequency
- Top customers by revenue
- New vs returning customers

---

### TC-084: Reports - Product Performance
**Requirement**: FR-028  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Generate Product Report

**Expected Results**:
- Best sellers shown
- Products with no sales
- Low stock products
- Revenue per product
- Views vs purchases

---

## Wishlist Test Cases

### TC-085: Wishlist - Add to Wishlist
**Requirement**: FR-029  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Preconditions**:
- User logged in

**Test Steps**:
1. View product
2. Click wishlist icon (heart)

**Expected Results**:
- Product added to wishlist
- Heart icon filled/highlighted
- Success message shown
- Wishlist count updated

---

### TC-086: Wishlist - View Wishlist
**Requirement**: FR-029  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Navigate to wishlist page

**Expected Results**:
- All wishlist items displayed
- Product details shown
- Stock availability indicated
- Add to cart option
- Remove option

---

### TC-087: Wishlist - Move to Cart
**Requirement**: FR-029  
**Priority**: Low  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View wishlist
2. Click "Add to Cart" on item

**Expected Results**:
- Item added to cart
- Removed from wishlist
- Cart updated
- Success message

---

## Localization Test Cases

### TC-088: Localization - Switch Language
**Requirement**: FR-030  
**Priority**: High  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Click language selector
2. Select Arabic
3. View page

**Expected Results**:
- UI text in Arabic
- RTL layout applied
- Direction switched
- Preference saved
- All pages in Arabic

---

### TC-089: Localization - Arabic Layout
**Requirement**: FR-030  
**Priority**: High  
**Type**: Functional  
**Automation**: Partial

**Test Steps**:
1. Switch to Arabic
2. Navigate through site

**Expected Results**:
- Text aligned right
- Navigation reversed
- Proper RTL support
- Icons mirrored correctly
- Dates in Arabic format

---

### TC-090: Localization - Product Content
**Requirement**: FR-030  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. View product in English
2. Switch to Arabic
3. View same product

**Expected Results**:
- Product name in Arabic
- Description in Arabic
- Specifications translated
- If no translation, fallback to English
- Currency localized

---

## Security Test Cases

### TC-096: Security - Password Hashing
**Requirement**: NFR-004  
**Priority': Critical  
**Type': Security  
**Automation**: Yes

**Test Steps**:
1. Create user account
2. Inspect database

**Expected Results**:
- Password hashed with bcrypt
- Cost factor >= 12
- Hash unique per user
- Original password not stored
- Cannot reverse engineer password

---

### TC-097: Security - JWT Token Security
**Requirement**: NFR-004  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Login and receive JWT
2. Inspect token
3. Attempt to modify token
4. Use modified token

**Expected Results**:
- Token properly signed
- Expiry set correctly (15 min)
- Token includes user ID, role
- Modified token rejected
- 401 Unauthorized on invalid token

---

### TC-098: Security - HTTPS Enforcement
**Requirement**: NFR-004  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Attempt HTTP request
2. Verify redirect

**Expected Results**:
- HTTP redirects to HTTPS
- 301 Permanent Redirect
- All traffic encrypted
- Cookies marked Secure

---

### TC-099: Security - Data Protection
**Requirement**: NFR-005  
**Priority**: Critical  
**Type**: Security  
**Automation': Yes

**Test Steps**:
1. Inspect database
2. Review logs
3. Check API responses

**Expected Results**:
- Sensitive data encrypted at rest
- PII not in logs
- Card data never stored locally
- API doesn't expose sensitive info
- GDPR compliant

---

### TC-100: Security - PCI Compliance
**Requirement**: NFR-005  
**Priority**: Critical  
**Type**: Security  
**Automation**: Partial

**Test Steps**:
1. Review payment flow
2. Check card data handling

**Expected Results**:
- Stripe handles card data
- No card data touches server
- PCI SAQ-A compliant
- Stripe Elements used
- Tokenization implemented

---

### TC-101: Security - Authorization
**Requirement**: NFR-006  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Attempt to access admin endpoint as Customer
2. Attempt to modify other user's order

**Expected Results**:
- 403 Forbidden response
- Access denied
- Audit log entry created
- Cannot escalate privileges

---

### TC-102: Security - SQL Injection
**Requirement**: NFR-006  
**Priority**: Critical  
**Type': Security  
**Automation**: Yes

**Test Steps**:
1. Attempt SQL injection in:
   - Login form: `admin' OR '1'='1`
   - Search: `'; DROP TABLE Products;--`

**Expected Results**:
- No SQL injection possible
- Parameterized queries used
- Input sanitized
- Error handled gracefully
- Security team notified

---

### TC-103: Security - XSS Prevention
**Requirement**: NFR-007  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Attempt XSS in:
   - Product review: `<script>alert('XSS')</script>`
   - Profile name: `<img src=x onerror=alert(1)>`

**Expected Results**:
- Scripts not executed
- Input encoded
- HTML sanitized
- Safe rendering

---

### TC-104: Security - CSRF Protection
**Requirement**: NFR-007  
**Priority**: Critical  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Attempt state-changing operation without CSRF token
2. Attempt with invalid token

**Expected Results**:
- Request rejected
- CSRF tokens required
- Tokens validated
- 403 Forbidden

---

### TC-105: Security - Rate Limiting
**Requirement**: NFR-007  
**Priority**: High  
**Type**: Security  
**Automation**: Yes

**Test Steps**:
1. Make 100 login requests in 1 minute

**Expected Results**:
- After 10 requests: 429 Too Many Requests
- Rate limit headers included
- Retry-After header present
- IP temporarily blocked

---

## Performance Test Cases

### TC-091: Performance - Page Load Time
**Requirement**: NFR-001  
**Priority**: High  
**Type**: Performance  
**Automation**: Yes

**Test Steps**:
1. Clear cache
2. Load homepage
3. Measure load time

**Expected Results**:
- Initial load: < 3 seconds
- Subsequent loads: < 1 second
- Lighthouse score: > 90

---

### TC-092: Performance - API Response Time
**Requirement**: NFR-001  
**Priority': High  
**Type**: Performance  
**Automation**: Yes

**Test Steps**:
1. Call various API endpoints
2. Measure response times

**Expected Results**:
- 90th percentile: < 500ms
- Median: < 200ms
- Search: < 1 second

---

### TC-093: Performance - Concurrent Users
**Requirement**: NFR-002  
**Priority**: High  
**Type': Performance  
**Automation**: Yes

**Test Steps**:
1. Simulate 1000 concurrent users
2. Monitor system performance

**Expected Results**:
- System handles load
- Response times acceptable
- No errors
- No crashes

---

### TC-094: Performance - Throughput
**Requirement**: NFR-002  
**Priority**: High  
**Type**: Performance  
**Automation**: Yes

**Test Steps**:
1. Generate 500 API requests/second
2. Monitor

**Expected Results**:
- System handles throughput
- No degradation
- Database performs well

---

### TC-095: Performance - Scalability
**Requirement': NFR-003  
**Priority**: Medium  
**Type**: Performance  
**Automation': Yes

**Test Steps**:
1. Add server instances
2. Test load balancing

**Expected Results**:
- Horizontal scaling works
- Load distributed evenly
- No single point of failure

---

## Additional Test Cases

### TC-106: Reliability - Uptime
**Requirement**: NFR-008  
**Priority**: High  
**Type**: Reliability  
**Automation': Partial

**Test Steps**:
1. Monitor uptime over 30 days

**Expected Results**:
- 99.9% uptime
- < 43 minutes downtime/month

---

### TC-107: Error Handling - User-Friendly Errors
**Requirement**: NFR-009  
**Priority**: Medium  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Trigger various errors
2. Check error messages

**Expected Results**:
- No technical details exposed
- User-friendly messages
- Helpful suggestions
- Support contact provided

---

### TC-108: Error Handling - Graceful Degradation
**Requirement**: NFR-009  
**Priority**: Medium  
**Type**: Reliability  
**Automation': Partial

**Test Steps**:
1. Disable Stripe temporarily
2. Attempt checkout

**Expected Results**:
- Graceful error message
- Alternative payment suggested
- Order saved for later
- User notified

---

### TC-109: Data Integrity - Transaction Handling
**Requirement**: NFR-010  
**Priority**: Critical  
**Type**: Functional  
**Automation**: Yes

**Test Steps**:
1. Place order (multi-step transaction)
2. Simulate failure mid-transaction

**Expected Results**:
- Transaction rolled back
- Database consistent
- No partial orders
- Inventory accurate

---

### TC-110: Data Integrity - Prevent Overselling
**Requirement**: NFR-010  
**Priority**: Critical  
**Type': Functional  
**Automation**: Yes

**Test Steps**:
1. Product has 1 item in stock
2. Two users add to cart simultaneously
3. Both attempt checkout

**Expected Results**:
- Only one order succeeds
- Second order fails: "Out of stock"
- Inventory accurate
- No overselling

---

### TC-111: Usability - Responsive Design
**Requirement**: NFR-011  
**Priority**: High  
**Type**: Usability  
**Automation**: Partial

**Test Steps**:
1. Test on mobile (375px)
2. Test on tablet (768px)
3. Test on desktop (1920px)

**Expected Results**:
- Responsive layout
- Mobile-friendly navigation
- Touch-friendly buttons
- Readable text
- No horizontal scroll

---

### TC-112: Usability - Accessibility
**Requirement**: NFR-011  
**Priority**: High  
**Type**: Usability  
**Automation': Partial

**Test Steps**:
1. Test keyboard navigation
2. Test screen reader
3. Run accessibility audit

**Expected Results**:
- WCAG 2.1 Level AA compliant
- Keyboard accessible
- Screen reader compatible
- Proper ARIA labels
- Color contrast sufficient

---

### TC-113: Documentation - API Documentation
**Requirement**: NFR-012  
**Priority**: Low  
**Type**: Documentation  
**Automation**: No

**Test Steps**:
1. Review API documentation
2. Test documented endpoints

**Expected Results**:
- Swagger/OpenAPI available
- All endpoints documented
- Examples provided
- Up-to-date
- Accurate

---

### TC-114: Code Quality - Test Coverage
**Requirement**: NFR-013  
**Priority**: Medium  
**Type**: Code Quality  
**Automation**: Yes

**Test Steps**:
1. Run test coverage report

**Expected Results**:
- Unit test coverage > 80%
- Integration tests for critical paths
- No critical code uncovered

---

### TC-115: Monitoring - Logging
**Requirement**: NFR-014  
**Priority': Medium  
**Type**: Operational  
**Automation**: Partial

**Test Steps**:
1. Trigger various actions
2. Review logs

**Expected Results**:
- All actions logged
- Structured JSON format
- Log levels correct
- Correlation IDs present
- No sensitive data in logs

---

## Test Summary

### Test Execution Tracking

| Module | Total | Pass | Fail | Blocked | Pass % |
|--------|-------|------|------|---------|--------|
| User Management | 15 | - | - | - | - |
| Product Catalog | 15 | - | - | - | - |
| Shopping Cart | 12 | - | - | - | - |
| Checkout & Orders | 18 | - | - | - | - |
| Admin Panel | 24 | - | - | - | - |
| Security | 10 | - | - | - | - |
| Performance | 5 | - | - | - | - |
| Localization | 3 | - | - | - | - |
| **Total** | **115** | - | - | - | - |

### Priority Breakdown

| Priority | Count |
|----------|-------|
| Critical | 18 |
| High | 52 |
| Medium | 35 |
| Low | 10 |

### Test Type Breakdown

| Type | Count |
|------|-------|
| Functional | 75 |
| Negative | 15 |
| Security | 10 |
| Performance | 5 |
| Usability | 5 |
| Others | 5 |

### Automation Status

| Status | Count | Percentage |
|--------|-------|------------|
| Automated | 95 | 82.6% |
| Partial | 15 | 13.0% |
| Manual | 5 | 4.4% |

---

## Test Environment Setup

### Prerequisites
- Staging environment URL
- Test database with sample data
- Stripe test account
- Email testing (Mailtrap/similar)
- Test user accounts (Customer, Admin, SuperAdmin)

### Test Data
- Sample products (50+)
- Sample categories (10+)
- Test users (various roles)
- Sample orders (various statuses)
- Discount codes
- Product reviews

---

## Appendix

### Test Status Legend
- ✅ **Pass**: Test executed successfully, all criteria met
- ❌ **Fail**: Test failed, defect logged
- ⏸️ **Blocked**: Cannot execute due to dependency
- ⏭️ **Skipped**: Intentionally not executed

### Bug Severity Levels
- **Critical**: System crash, data loss, security breach
- **High**: Major feature broken, no workaround
- **Medium**: Feature partially broken, workaround exists
- **Low': Cosmetic issue, minor inconvenience

### Defect Template
```
Title: [Component] - Brief description
Severity: Critical/High/Medium/Low
Priority: P1/P2/P3/P4
Requirement: FR-XXX
Test Case: TC-XXX

Steps to Reproduce:
1. Step 1
2. Step 2

Expected Result:
[What should happen]

Actual Result:
[What actually happens]

Environment:
- Browser: Chrome 120
- OS: Windows 11
- URL: https://staging.example.com

Screenshots: [Attached]
```

---

**Document Status**: ✅ Ready for Test Execution

**Next Steps**:
1. QA Team: Set up test environment
2. QA Team: Prepare test data
3. QA Team: Execute test cases
4. QA Team: Report defects
5. Dev Team: Fix critical/high priority bugs
6. QA Team: Regression testing
