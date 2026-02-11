# GitHub Issue Tracking Workflow

## Overview
Each agent must create GitHub issues to track their work and close them upon completion. This provides transparency and progress tracking across the multi-agent system.

---

## 🎯 Agent Workflow

### **Step 1: Create Issue When Starting**

```bash
# Navigate to project directory
cd D:\source\ecommerce-multiagent-project

# Create issue for your agent tasks
gh issue create \
  --title "[AgentName] Task Title" \
  --body "$(cat <<EOF
## Agent: [Agent Name]

## Tasks
- [ ] Task 1
- [ ] Task 2
- [ ] Task 3

## Deliverables
- File 1
- File 2

## Dependencies
- Depends on: #issue_number (if any)

## Status
- Started: $(date +%Y-%m-%d)
EOF
)" \
  --label "agent-task,in-progress"

# Save the issue number
# Example output: Created issue #1
```

**PowerShell Version:**
```powershell
# Create issue
gh issue create `
  --title "[PM] Create Requirements and API Specification" `
  --body "Agent: Project Manager`n`nTasks:`n- [ ] Create requirements.md`n- [ ] Create api-specification.md`n- [ ] Define test cases" `
  --label "agent-task,in-progress"
```

---

### **Step 2: Update Issue During Work**

```bash
# Add comment to update progress
gh issue comment <issue-number> --body "✅ Completed: [task name]"

# Or update with checklist
gh issue comment <issue-number> --body "Progress update:
- ✅ Task 1 complete
- 🔄 Task 2 in progress
- ⏳ Task 3 pending"
```

---

### **Step 3: Close Issue When Complete**

```bash
# Close issue with completion message
gh issue close <issue-number> --comment "✅ All tasks completed. Deliverables:
- requirements.md
- api-specification.md
- test-cases.md

Ready for next agent."
```

---

## 📋 Issue Templates by Agent

### **1. Project Manager**
```bash
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

### **2. Database Engineer**
```bash
gh issue create \
  --title "[DB] Setup PostgreSQL Schema and Migrations" \
  --body "## Agent: Database Engineer

## Tasks
- [ ] Create Fluent Migrator migrations for all tables
- [ ] Configure LLBLGen Pro project
- [ ] Generate entities from schema
- [ ] Create database views
- [ ] Setup indexes for performance

## Deliverables
- Migration files (V1_*.cs, V2_*.cs, etc.)
- LLBLGen Pro project files
- Generated entities
- Database documentation

## Dependencies
- Depends on: #1 (PM requirements)

## Acceptance Criteria
- [ ] All migrations run successfully
- [ ] LLBLGen entities generated
- [ ] Database normalized (3NF minimum)
- [ ] Indexes on foreign keys" \
  --label "agent-task,database,in-progress"
```

### **3. Backend Developer**
```bash
gh issue create \
  --title "[Backend] Build ASP.NET Core API with Feature Folders" \
  --body "## Agent: Backend Developer

## Tasks
- [ ] Setup 3-project structure (API, Core, Data)
- [ ] Create feature folders with repositories
- [ ] Implement services with business logic
- [ ] Build controllers with endpoints
- [ ] Configure AutoMapper profiles
- [ ] Setup localization (en/ar)
- [ ] Add SK framework to Misc folder

## Deliverables
- ECommerce.API project
- ECommerce.Core project (with features)
- ECommerce.Data project (with LLBLGen integration)
- SK framework utilities

## Dependencies
- Depends on: #2 (DB schema and entities)

## Acceptance Criteria
- [ ] All API endpoints working
- [ ] Repository pattern implemented
- [ ] DTOs separated from entities
- [ ] Localization working
- [ ] Unit tests passing" \
  --label "agent-task,backend,in-progress"
```

### **4. Frontend Developer**
```bash
gh issue create \
  --title "[Frontend] Build React UI with Tailwind" \
  --body "## Agent: Frontend Developer

## Tasks
- [ ] Setup Vite + React + TypeScript project
- [ ] Configure Tailwind CSS
- [ ] Implement React Query for API calls
- [ ] Setup Zustand for state management
- [ ] Build reusable components
- [ ] Create pages (Home, Products, Cart, Checkout, etc.)
- [ ] Implement authentication flow

## Deliverables
- React application in /frontend directory
- Component library
- API integration layer
- Responsive UI

## Dependencies
- Depends on: #3 (Backend API)
- Can work in parallel if using mock data

## Acceptance Criteria
- [ ] All pages responsive
- [ ] API integration working
- [ ] Authentication implemented
- [ ] Cart functionality complete" \
  --label "agent-task,frontend,in-progress"
```

### **5. QA Tester**
```bash
gh issue create \
  --title "[QA] Write Tests and Validate System" \
  --body "## Agent: QA Tester

## Tasks
- [ ] Write unit tests (xUnit)
- [ ] Write integration tests (WebApplicationFactory)
- [ ] Write E2E tests (Playwright)
- [ ] Validate against test cases from PM
- [ ] Create test data fixtures
- [ ] Document test coverage

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
- [ ] All test cases from PM validated
- [ ] CI/CD pipeline runs tests" \
  --label "agent-task,testing,in-progress"
```

### **6. DevOps Engineer**
```bash
gh issue create \
  --title "[DevOps] Setup CI/CD and Deployment" \
  --body "## Agent: DevOps Engineer

## Tasks
- [ ] Create Dockerfiles (API, Frontend, Database)
- [ ] Create docker-compose.yml
- [ ] Setup GitHub Actions workflow
- [ ] Configure Kubernetes manifests
- [ ] Setup environment variables
- [ ] Create deployment scripts

## Deliverables
- Dockerfile for each service
- docker-compose.yml
- .github/workflows/ci-cd.yml
- kubernetes/ directory with manifests
- Deployment documentation

## Dependencies
- Depends on: #5 (Tests must pass)

## Acceptance Criteria
- [ ] Docker images build successfully
- [ ] docker-compose up works locally
- [ ] GitHub Actions runs tests on PR
- [ ] Kubernetes deployment configured
- [ ] Environment variables documented" \
  --label "agent-task,devops,in-progress"
```

---

## 🏷️ Label Convention

- `agent-task` - All agent work items
- `in-progress` - Currently being worked on
- `blocked` - Waiting on dependencies
- `review-needed` - Ready for review
- Agent-specific: `pm`, `frontend`, `backend`, `database`, `qa`, `devops`
- Priority: `priority-high`, `priority-medium`, `priority-low`

---

## 📊 Tracking Progress

### View All Agent Issues
```bash
# List all open agent tasks
gh issue list --label "agent-task"

# View specific agent's tasks
gh issue list --label "agent-task,backend"

# View completed tasks
gh issue list --state closed --label "agent-task"
```

### Check Dependencies
```bash
# View specific issue to check dependencies
gh issue view <issue-number>
```

---

## 🔄 Example Complete Workflow

```bash
# 1. Project Manager starts
gh issue create --title "[PM] Create Requirements" --body "..." --label "agent-task,pm,in-progress"
# Issue #1 created

# 2. PM completes work
gh issue close 1 --comment "✅ Requirements complete. Ready for Database Engineer."

# 3. Database Engineer starts (depends on #1)
gh issue create --title "[DB] Setup Schema" --body "Depends on: #1..." --label "agent-task,database,in-progress"
# Issue #2 created

# 4. DB Engineer updates progress
gh issue comment 2 --body "✅ Migrations created. Running LLBLGen..."

# 5. DB Engineer completes
gh issue close 2 --comment "✅ Schema complete. Entities generated. Ready for Backend."

# ... and so on
```

---

## 🚀 Quick Start Commands

### **For Each Agent:**

**1. Start Work:**
```bash
# Create your issue (use template above)
gh issue create --title "[AgentName] Your Task" --body "..." --label "agent-task,in-progress"
```

**2. Update Progress:**
```bash
# Add comments as you complete tasks
gh issue comment <issue-number> --body "✅ Completed: [task]"
```

**3. Finish Work:**
```bash
# Close issue when done
gh issue close <issue-number> --comment "✅ All complete. Files: [list]"
```

---

## 📝 Notes

- **Always check dependencies** before starting work
- **Update issue comments** to show progress (helps other agents)
- **Close issues promptly** to unblock dependent agents
- **Reference issue numbers** in commit messages: `git commit -m "feat: add areas endpoint (#3)"`
- **Use issue numbers in PRs** if using feature branches

---

## 🔗 Useful Links

- [GitHub CLI Issue Docs](https://cli.github.com/manual/gh_issue)
- [Issue Labels Guide](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels)
- [Linking Issues to PRs](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue)
