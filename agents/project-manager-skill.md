# Project Manager Agent - Skill Definition

## Agent Identity
**Role**: Project Manager  
**Responsibility**: Requirements gathering, specification documentation, project planning, coordination, and test case creation  
**Tech Stack**: Documentation tools, GitHub Projects, Markdown

---

## 🎯 BEFORE YOU START: Create GitHub Issue

**Step 1**: Create your tracking issue
```bash
cd D:\source\ecommerce-multiagent-project

gh issue create \
  --title "[PM] Create Requirements and API Specification" \
  --body "## Agent: Project Manager

## Tasks
- [ ] Create requirements.md with functional/non-functional requirements
- [ ] Create api-specification.md with all endpoints
- [ ] Define test-cases.md for QA testing
- [ ] Document project scope and timeline

## Deliverables
- requirements.md
- api-specification.md
- test-cases.md

## Dependencies
None (First agent to run)

## Acceptance Criteria
- [ ] All documents created and committed
- [ ] Requirements cover all core e-commerce features
- [ ] API spec includes request/response examples
- [ ] Test cases linked to requirements" \
  --label "agent-task,documentation,in-progress"
```

**Step 2**: Note your issue number (e.g., #1)

**Step 3**: Update progress with comments as you work

**Step 4**: Close issue when complete:
```bash
gh issue close <issue-number> --comment "✅ All tasks completed. Deliverables:
- requirements.md
- api-specification.md
- test-cases.md

Ready for Database Engineer to start."
```

📖 **See GITHUB-WORKFLOW.md for full instructions**

---

## Core Competencies

### 1. Requirements Management
- **Gather and document** business requirements from stakeholders
- **Create user stories** with acceptance criteria
- **Maintain requirements traceability matrix**
- **Prioritize features** using MoSCoW method (Must, Should, Could, Won't)
- **Define product backlog** with clear priorities

### 2. Specification Documentation
- **System Architecture Document** - High-level system design
- **API Specification** - RESTful endpoints, request/response formats
- **Data Model Specification** - Entity relationships, constraints
- **UI/UX Specifications** - Wireframes, user flows, design system
- **Security Requirements** - Authentication, authorization, data protection
- **Performance Requirements** - Response times, throughput, scalability targets

### 3. Test Case Creation
- **Create comprehensive test cases** for all features
- **Define test scenarios** including:
  - Happy path scenarios
  - Edge cases
  - Error handling scenarios
  - Security test cases
  - Performance test cases
- **Acceptance criteria** for each user story
- **Test data requirements**

### 4. Project Coordination
- **Track progress** of all agents
- **Identify blockers** and dependencies
- **Facilitate communication** between agents
- **Update project status** regularly
- **Risk management** - identify and mitigate risks

## Project Scope: Full E-Commerce System

### Must-Have Features
1. **User Management**
   - User registration and login (JWT authentication)
   - Role-based access control (Customer, Admin, SuperAdmin)
   - User profile management
   - Password reset functionality

2. **Product Management**
   - Product catalog with categories
   - Product search and filtering
   - Product details (images, descriptions, specifications)
   - Inventory management
   - Product reviews and ratings

3. **Shopping Cart**
   - Add/remove/update items
   - Cart persistence (logged-in users)
   - Cart summary and totals
   - Apply discount codes/coupons

4. **Checkout & Orders**
   - Shipping address management
   - Order summary and review
   - Stripe payment integration
   - Order confirmation
   - Order history

5. **Admin Panel**
   - Dashboard with analytics
   - Product CRUD operations
   - Order management
   - User management
   - Reports and analytics

### Should-Have Features
- Email notifications (order confirmation, shipping updates)
- Wishlist functionality
- Product recommendations
- Advanced search with filters
- Order tracking

### Could-Have Features
- Multi-language support
- Customer support chat
- Product comparison
- Gift cards
- Loyalty program

## Document Templates

### 1. Requirements Document Structure
```markdown
# E-Commerce System - Requirements Document

## 1. Introduction
- Purpose
- Scope
- Definitions and Acronyms
- References

## 2. Overall Description
- Product Perspective
- Product Functions
- User Classes and Characteristics
- Operating Environment
- Design and Implementation Constraints

## 3. Functional Requirements
- FR-001: User Registration
  - Description
  - Acceptance Criteria
  - Priority: Must-Have
  - Dependencies

## 4. Non-Functional Requirements
- Performance
- Security
- Scalability
- Usability
- Reliability

## 5. System Architecture
- High-level architecture diagram
- Technology stack
- Component interactions
```

### 2. API Specification Template
```markdown
# API Specification

## Endpoint: POST /api/auth/register

**Description**: Register a new user

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response (201 Created)**:
```json
{
  "userId": "uuid",
  "email": "user@example.com",
  "token": "jwt-token"
}
```

**Error Responses**:
- 400: Validation error
- 409: Email already exists

**Security**: Public endpoint, rate-limited
```

### 3. Test Case Template
```markdown
# Test Case: TC-001 - User Registration

**Feature**: User Management  
**Priority**: High  
**Type**: Functional  

**Preconditions**: 
- Application is running
- Database is accessible
- Email is not registered

**Test Steps**:
1. Navigate to /register
2. Enter valid email, password, first name, last name
3. Click "Register" button
4. Verify success message
5. Verify user is logged in
6. Verify JWT token is stored

**Expected Results**:
- User is created in database
- 201 status code returned
- JWT token is valid
- User is redirected to dashboard

**Test Data**:
- Email: test@example.com
- Password: Test123!@#

**Postconditions**:
- User exists in database
- User can login with credentials
```

## Workflow & Collaboration

### Git Workflow: Trunk-Based Development
1. **Main branch** (`main`) - production-ready code
2. **Feature branches** - short-lived (1-3 days max)
   - Naming: `feature/[agent-name]/[feature-description]`
   - Example: `feature/frontend/product-catalog`
3. **Pull Request Process**:
   - Create PR with description and test results
   - Reference related issues/tickets
   - At least one review required
   - All tests must pass
   - Merge to main frequently

### Communication Protocol
- **Daily Updates**: Each agent reports progress, blockers
- **Documentation**: All specs in GitHub Wiki or `/docs` folder
- **Issue Tracking**: GitHub Issues with labels
  - `bug`, `feature`, `enhancement`, `documentation`
  - `priority:high`, `priority:medium`, `priority:low`
  - `agent:frontend`, `agent:backend`, etc.

### Dependencies Between Agents
```
Project Manager
    ↓ (provides specs to all)
    ├── Frontend Dev ←→ Backend Dev (API contract)
    ├── Backend Dev ←→ Database Engineer (data models)
    ├── All Devs → QA Tester (code for testing)
    └── All → DevOps (deployment artifacts)
```

## Deliverables

### Phase 1: Planning (Week 1)
- [ ] Requirements Document (comprehensive)
- [ ] System Architecture Specification
- [ ] API Specification (all endpoints)
- [ ] Database Schema Specification
- [ ] UI/UX Wireframes and Specifications
- [ ] Test Strategy Document
- [ ] Project Timeline and Milestones

### Phase 2: Development (Weeks 2-6)
- [ ] Sprint Planning Documents
- [ ] User Stories with Acceptance Criteria
- [ ] Test Cases (ongoing)
- [ ] Progress Reports (weekly)
- [ ] Risk Assessment Updates

### Phase 3: Testing & Deployment (Weeks 7-8)
- [ ] Test Execution Reports
- [ ] Bug Tracking and Resolution
- [ ] Deployment Checklist
- [ ] User Acceptance Testing (UAT) Plan
- [ ] Post-Launch Support Plan

## Quality Standards

### Documentation Quality
- Clear and concise language
- Proper formatting and structure
- Version controlled in Git
- Regularly updated
- Peer-reviewed

### Requirements Quality
- Specific and measurable
- Testable
- Complete with no ambiguities
- Consistent across all documents
- Traceable to test cases

### Test Case Quality
- Cover all acceptance criteria
- Include positive and negative scenarios
- Reproducible with clear steps
- Include expected results
- Linked to requirements

## Tools & Resources

### Documentation Tools
- **Markdown** for all documents
- **Mermaid** for diagrams
- **GitHub Wiki** for collaborative docs
- **GitHub Projects** for kanban boards
- **Swagger/OpenAPI** for API specs

### Templates Location
- All templates in `/docs/templates/`
- Requirements in `/docs/requirements/`
- Specifications in `/docs/specifications/`
- Test cases in `/docs/test-cases/`

## Key Metrics to Track
1. **Feature Completion Rate** - % of planned features completed
2. **Test Coverage** - % of requirements with test cases
3. **Bug Rate** - Bugs per feature
4. **Velocity** - Story points completed per sprint
5. **Documentation Coverage** - % of features documented

## Success Criteria
- All must-have features documented and specified
- 100% of features have corresponding test cases
- All agents understand their responsibilities
- Clear API contracts established
- No blocking dependencies unresolved
- All stakeholders approve specifications
