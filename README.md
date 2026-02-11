# E-Commerce Multi-Agent System

A comprehensive multi-agent system for building a full-stack e-commerce platform using React, Tailwind CSS, ASP.NET Core 10, LLBLGen Pro, Fluent Migrator, and PostgreSQL.

## Project Overview

This project uses a **multi-agent approach** where each specialized agent is responsible for a specific aspect of the e-commerce system. Each agent has a detailed skill.md file defining their responsibilities, technical stack, coding standards, and deliverables.

## Technology Stack

### Frontend
- **Framework**: React 18+ with TypeScript
- **Styling**: Tailwind CSS
- **State Management**: React Query (TanStack Query) + Zustand
- **Build Tool**: Vite
- **Testing**: Vitest, Playwright

### Backend
- **Framework**: ASP.NET Core 10 (C# 12)
- **Architecture**: Feature-based (vertical slices)
- **ORM**: LLBLGen Pro (Adapter pattern)
- **Migrations**: Fluent Migrator
- **Authentication**: JWT tokens
- **Payment**: Stripe integration

### Database
- **Database**: PostgreSQL 16+
- **Migrations**: Fluent Migrator (version-based)
- **Schema**: Normalized (3NF)

### DevOps
- **Containerization**: Docker, Docker Compose
- **Orchestration**: Kubernetes
- **CI/CD**: GitHub Actions
- **Deployment**: Rolling updates, auto-scaling

## Agent Roles

### 1. Project Manager
**Skill File**: `agents/project-manager-skill.md`

**Responsibilities**:
- Requirements gathering and documentation
- API and system specifications
- Test case creation
- Project coordination
- Risk management

**Key Deliverables**:
- Requirements document
- System architecture specification
- API specification (all endpoints)
- Test strategy document
- Project timeline and milestones

---

### 2. Frontend Developer
**Skill File**: `agents/frontend-developer-skill.md`

**Responsibilities**:
- React application development
- Tailwind CSS styling
- React Query integration
- Zustand state management
- Component testing

**Key Deliverables**:
- Responsive React application
- Reusable component library
- API integration
- Unit and E2E tests
- Accessibility compliance

**Tech Stack**:
- React 18, TypeScript, Tailwind CSS
- React Query, Zustand, React Router
- Vitest, Playwright

---

### 3. Backend Developer
**Skill File**: `agents/backend-developer-skill.md`

**Responsibilities**:
- ASP.NET Core API development
- Feature-based architecture
- LLBLGen Pro integration
- Repository and service implementation
- Localization (en/ar)

**Key Deliverables**:
- RESTful API (all endpoints)
- Feature folders (Area, Product, Order, etc.)
- Repository implementations
- Service layer with business logic
- AutoMapper profiles
- Localization JSON files

**Project Structure**:
```
ECommerce.sln
├── ECommerce.API/          (Controllers, Middleware)
├── ECommerce.Core/         (Features, Resources, Misc)
│   ├── Features/
│   │   ├── Area/
│   │   │   ├── IAreaRepository.cs
│   │   │   ├── AreaRepository.cs
│   │   │   ├── AreaService.cs
│   │   │   ├── AreaFilter.cs
│   │   │   ├── AreaInputs.cs
│   │   │   └── AreaOutputs.cs
│   │   ├── Product/
│   │   └── ... (other features)
│   ├── Resources/          (Localization: en.json, ar.json)
│   └── Misc/               (SK framework - to be added)
└── ECommerce.Data/         (LLBLGen entities, Migrations)
```

---

### 4. Database Engineer
**Skill File**: `agents/database-engineer-skill.md`

**Responsibilities**:
- PostgreSQL database design
- Fluent Migrator scripts
- LLBLGen Pro coordination
- Database views
- Performance optimization

**Key Deliverables**:
- Complete database schema
- Fluent Migrator scripts (sequential)
- Database views
- Seed data migrations
- LLBLGen Pro project configuration
- Performance indexes

**Naming Conventions**:
- Tables: `lowercase_with_underscores` (plural)
- Columns: `lowercase_with_underscores`
- Migrations: `VXXX_DescriptiveName.cs`

---

### 5. QA Tester
**Skill File**: `agents/qa-tester-skill.md`

**Responsibilities**:
- Test strategy and planning
- Unit testing (xUnit, Vitest)
- Integration testing
- E2E testing (Playwright)
- Performance and security testing

**Key Deliverables**:
- Unit test suites (80%+ coverage)
- Integration test suites
- E2E test scenarios
- Performance test scripts
- Security test checklist
- Bug reports

**Testing Stack**:
- Backend: xUnit, Moq, FluentAssertions
- Frontend: Vitest, Testing Library, Playwright
- Performance: K6

---

### 6. DevOps Engineer
**Skill File**: `agents/devops-engineer-skill.md`

**Responsibilities**:
- Docker containerization
- Kubernetes orchestration
- GitHub Actions CI/CD
- Deployment automation
- Monitoring and logging

**Key Deliverables**:
- Dockerfiles (multi-stage builds)
- Docker Compose (local dev)
- Kubernetes manifests
- GitHub Actions workflows
- Deployment scripts
- SSL/TLS configuration

**Infrastructure**:
- Docker + Docker Compose (local)
- Kubernetes (production)
- GitHub Container Registry
- GitHub Actions (CI/CD)

---

## Project Features

### Must-Have Features
1. **User Management**
   - Registration/Login (JWT)
   - Role-based access (Customer, Admin, SuperAdmin)
   - Profile management

2. **Product Management**
   - Product catalog with categories
   - Search and filtering
   - Product reviews

3. **Shopping Cart**
   - Add/remove/update items
   - Cart persistence

4. **Checkout & Orders**
   - Shipping address
   - Stripe payment
   - Order history

5. **Admin Panel**
   - Dashboard
   - Product/Order/User management

## Git Workflow

**Branch Naming**:
- `feature/[agent-name]/[feature-name]`
- Example: `feature/frontend/product-catalog`, `feature/backend/user-auth`

**Trunk-Based Development**:
- Main branch: `main` (production-ready)
- Feature branches: short-lived (1-3 days)
- Frequent merges to main
- Pull requests required

**Commit Messages**:
- `feat: add user authentication`
- `fix: resolve cart state bug`
- `docs: update API documentation`

## Getting Started

### Prerequisites
- **Frontend**: Node.js 20+, npm
- **Backend**: .NET SDK 8.0+
- **Database**: PostgreSQL 16+
- **Tools**: Docker, Git, LLBLGen Pro

### Local Development

#### 1. Clone Repository
```bash
git clone https://github.com/your-org/ecommerce-multiagent.git
cd ecommerce-multiagent
```

#### 2. Start with Docker Compose
```bash
docker-compose up -d
```

This starts:
- PostgreSQL database (port 5432)
- Backend API (port 5000)
- Frontend (port 3000)

#### 3. Run Migrations
```bash
cd backend
dotnet run --project ECommerce.API -- migrate up
```

#### 4. Access Application
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000/api
- **Swagger**: http://localhost:5000/swagger

### Manual Setup (Without Docker)

#### Database
```bash
# Create database
createdb -U postgres ecommerce

# Run migrations
cd backend
dotnet run --project ECommerce.API -- migrate up
```

#### Backend
```bash
cd backend
dotnet restore
dotnet run --project src/ECommerce.API
```

#### Frontend
```bash
cd frontend
npm install
npm run dev
```

## Documentation

Each agent has a comprehensive skill file in the `agents/` directory:

- `project-manager-skill.md` - Requirements, specifications, test cases
- `frontend-developer-skill.md` - React, Tailwind, state management
- `backend-developer-skill.md` - ASP.NET Core, LLBLGen Pro, feature architecture
- `database-engineer-skill.md` - PostgreSQL, Fluent Migrator, schema design
- `qa-tester-skill.md` - Testing strategy, automation, quality gates
- `devops-engineer-skill.md` - Docker, Kubernetes, CI/CD

## Project Structure

```
ecommerce-multiagent-project/
├── agents/                      # Agent skill definitions
│   ├── project-manager-skill.md
│   ├── frontend-developer-skill.md
│   ├── backend-developer-skill.md
│   ├── database-engineer-skill.md
│   ├── qa-tester-skill.md
│   └── devops-engineer-skill.md
├── frontend/                    # React application (to be created)
├── backend/                     # ASP.NET Core API (to be created)
│   ├── src/
│   │   ├── ECommerce.API/
│   │   ├── ECommerce.Core/
│   │   └── ECommerce.Data/
│   └── tests/
├── tests/                       # Test suites (to be created)
├── kubernetes/                  # K8s manifests (to be created)
├── docker/                      # Dockerfiles (to be created)
├── .github/workflows/           # GitHub Actions (to be created)
└── README.md                    # This file
```

## Localization

The application supports **English (en)** and **Arabic (ar)**.

**Resource Files**:
- `ECommerce.Core/Resources/Common/en.json` - Common English resources
- `ECommerce.Core/Resources/Common/ar.json` - Common Arabic resources
- `ECommerce.Core/Resources/Features/[Feature].en.json` - Feature-specific

## Contributing

Each agent should:
1. Work on their designated feature branch
2. Follow coding standards in their skill file
3. Write tests for new code
4. Create pull requests for review
5. Ensure all tests pass before merging

## Success Criteria

- [ ] All agent skill files reviewed and approved
- [ ] Project structure created
- [ ] Development environment setup
- [ ] All must-have features implemented
- [ ] 80%+ test coverage
- [ ] API documentation complete
- [ ] Deployment pipelines working
- [ ] Application accessible in production

## Support

For questions or issues:
1. Check the relevant agent skill file
2. Review project documentation
3. Create GitHub issue with appropriate label

## License

[Your License Here]

---

**Built with a Multi-Agent Architecture** | Each agent is a specialist in their domain, working together to deliver a high-quality e-commerce platform.
