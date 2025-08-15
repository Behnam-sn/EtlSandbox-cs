# ETL Sandbox Improvement Tasks Checklist

A logically ordered, actionable checklist to improve architecture, reliability, performance, security, and developer experience across the solution. Each item is intended to be small enough to plan/track, and all start with a check box.

1. [ ] Establish Architecture Decision Records (ADRs) for ETL topology, data flow, and infra boundaries
2. [ ] Document current solution architecture (Contexts: Application, Domain, Infrastructure, Hosts, Presentations) with diagrams
3. [ ] Define clear module boundaries and ownership for each Infrastructure (Jupiter, Mars, Neptune, Venus, Apollo)
4. [ ] Introduce a cohesive ETL pipeline abstraction (Extractor → Transformer → Loader → Synchronizer) interfaces in Application layer
5. [ ] Ensure Domain remains persistence-agnostic and free of infrastructure dependencies (verify references and enforce with analyzers)
6. [ ] Consolidate duplicate EF entity configurations for CustomerOrderFlat across Mars/Neptune/Venus (create shared configuration where appropriate)
7. [ ] Create a consistent DbContext registration pattern (AddDbContextPool, retries, command timeout, migrations assembly) for all providers
8. [ ] Add a centralized configuration for connection resiliency (EnableRetryOnFailure policy, exponential backoff) and timeouts
9. [ ] Introduce Options validation (IValidateOptions) for GlobalSettings and worker settings, including required fields and ranges
10. [ ] Enforce strict nullability (enable <Nullable>enable</Nullable>) across all projects and fix nullable warnings
11. [ ] Introduce code analyzers (Roslyn, StyleCop/EditorConfig rules) and treat warnings as errors in CI
12. [ ] Add unit tests for Extractors, Transformers, Loaders, Synchronizers with fakes and known datasets
13. [ ] Add integration tests with Testcontainers (MySQL, SQL Server) or LocalDB to verify end-to-end ETL paths
14. [ ] Add smoke tests for Hosts (worker/web) to validate DI composition and basic startup
15. [ ] Implement health checks for external dependencies (DB connections) and expose endpoints where applicable (Web APIs)
16. [ ] Add structured logging (Serilog or Microsoft.Extensions.Logging with scopes) and correlation IDs across pipeline steps
17. [ ] Add OpenTelemetry for traces and metrics (spans per ETL step; DB deps as dependencies)
18. [ ] Standardize exception handling and error taxonomy for ETL steps; map to retryable vs non-retryable errors
19. [ ] Implement idempotency in loaders/synchronizers (natural keys, upserts, checksum/hash-based comparisons)
20. [ ] Add batching and back-pressure controls in workers (configurable batch size, degree of parallelism, bounded channels)
21. [ ] Ensure cancellation tokens are plumbed through extract/transform/load operations and honored in loops
22. [ ] Add transient-fault handling policies with Polly (retries with jitter, circuit breaker) for Dapper/ADO operations
23. [ ] Review transaction boundaries for loaders and synchronizers; implement appropriate isolation and consistency strategy
24. [ ] Optimize bulk operations (SqlBulkCopy settings, table locks, batch size, timeout) and document required SQL indexes
25. [ ] Create and manage EF Core migrations per infrastructure assembly; verify migrations assembly is set consistently
26. [ ] Add database schema versioning/compat checks between source and destination to catch drift early
27. [ ] Add data validation rules pre/post transform (schema checks, numeric ranges, required fields) with metrics on failures
28. [ ] Implement dead-letter handling for failed records (persist payload + error details for later reprocessing)
29. [ ] Add configurable scheduling for workers (cron/periodic timer) separate from Host lifetimes
30. [ ] Externalize and document appsettings.* conventions (per-environment files; secrets via user-secrets/KeyVault)
31. [ ] Perform security review for connection strings and credentials (no hardcoding; use secure stores; encrypt at rest)
32. [ ] Validate least-privilege database principals for source/destination access; document required permissions
33. [ ] Add input sanitization for raw SQL/Dapper queries; parameterize all queries; enable EF Core command interception logs
34. [ ] Add PII handling policy and masking/redaction for logs and error messages
35. [ ] Add guard clauses and argument validation for public APIs and constructors (throw helpful exceptions)
36. [ ] Normalize naming conventions across solution (folders, namespaces, class names) and enforce with analyzers
37. [ ] Remove dead code and unused dependencies; enable trimming analysis where feasible
38. [ ] Introduce a consistent DTO/record type for inter-step payloads with versioning strategy
39. [ ] Create performance benchmarks (BenchmarkDotNet) for hot paths (transformations, bulk copy) to quantify improvements
40. [ ] Add caching where appropriate (lookup tables, reference data) with invalidation strategy
41. [ ] Implement feature flags for risky changes (e.g., new loader version) to allow safe rollouts
42. [ ] Add Host-level graceful shutdown improvements (drain in-flight batches, flush logs/telemetry)
43. [ ] Ensure Hosted Services use BackgroundService with robust try/catch, logging, and retry sleeps/backoffs
44. [ ] Centralize DI registrations into extension methods per layer and ensure Hosts compose via these extensions only
45. [ ] Replace manual Mediator registration with MediatR service registration extension (services.AddMediatR([...]))
46. [ ] Add compile-time checks for circular dependencies between projects (dependency graph validation script)
47. [ ] Add solution-level Directory.Build.props/targets to unify versions, LangVersion, Nullable, analyzers, warnings as errors
48. [ ] Introduce consistent serialization settings (System.Text.Json) and date/time handling (UTC everywhere) guidelines
49. [ ] Add timeouts and command behaviors for EF Core (context.Database.SetCommandTimeout) and Dapper commands
50. [ ] Enable DbContext pooling where concurrency permits and verify thread-safety of usage patterns
51. [ ] Consolidate connection factory implementations and ensure proper disposal and pooling
52. [ ] Add EF Core logging filters to reduce noise; log slow queries with thresholds
53. [ ] Ensure ApplyConfigurationsFromAssembly usage meets conventions; add tests to catch missing IEntityTypeConfiguration
54. [ ] Create sample data seeders for local development and integration tests
55. [ ] Add CI pipeline (GitHub Actions/Azure Pipelines): build, test (unit+integration), code coverage, analyzers, artifacts
56. [ ] Enforce code coverage threshold in CI and publish coverage reports
57. [ ] Add PR templates and CODEOWNERS to drive review quality and ownership boundaries
58. [ ] Provide developer onboarding guide (how to run, env vars, DB setup, troubleshooting)
59. [ ] Add runtime configuration docs for each Host (env vars, connection strings, feature flags)
60. [ ] Add operational runbooks for incident handling (DB outage, schema drift, failed batches)
61. [ ] Implement consistent Soft Delete strategy for destination entities (flags, archival policy, cascades)
62. [ ] Introduce consistent date/time provider abstraction and inject for testability
63. [ ] Add centralized mapping layer (e.g., Mapster/AutoMapper) or document manual mapping conventions
64. [ ] Review and unify repository abstractions; avoid duplicating EF vs Dapper patterns without rationale
65. [ ] Add guardrails for large transactions (chunking, memory usage caps) and measure memory overhead
66. [ ] Add end-to-end observability dashboards (latency, throughput, failure rate, retries, backlog size)
67. [ ] Add SLA/SLO definitions and alerts based on metrics (error rate, lag behind source)
68. [ ] Verify thread-safety for shared singletons (resolvers); add tests and documentation
69. [ ] Introduce versioned loaders/synchronizers (V1, V2) with toggle to swap implementations safely
70. [ ] Implement data lineage tracking (record provenance: source, transform version, load timestamp)
71. [ ] Add validation of appsettings schema at startup and fail-fast with actionable messages
72. [ ] Ensure build outputs are deterministic and reproducible (lock package versions, restore caching)
73. [ ] Add license and third-party notices if distributing binaries; verify compliance of used packages
74. [ ] Add periodic dependency update workflow (Dependabot/Renovate) and changelog maintenance
75. [ ] Validate that Hosted Services are idempotent on restart (resume markers/checkpoints)
76. [ ] Introduce checkpointing mechanism for ETL (watermarks per table/stream) with storage and recovery
77. [ ] Add multi-tenant/environment support if applicable (separate schemas/databases) with safe configuration
78. [ ] Implement data anonymization/sanitization for non-production environments and test datasets
79. [ ] Add feature to simulate failures for chaos testing (fault injection) and verify resilience
80. [ ] Perform load testing and capacity planning; document scaling approach (horizontal workers, partitioning)
81. [ ] Add consistent result types for commands/handlers (success/failure with error details)
82. [ ] Create a common Abstractions project for cross-layer contracts to reduce cross-references
83. [ ] Review and standardize naming of connection strings (Source, Destination, etc.) across all Hosts and docs
84. [ ] Add validation and tests for starting point resolvers (Insert/SoftDelete) and their time/offset logic
85. [ ] Ensure Dapper queries use async APIs and streaming where appropriate to reduce memory spikes
86. [ ] Add pagination/windowing for large extracts (by key/time) to avoid long-running queries and locks
87. [ ] Implement poison message handling semantics for records consistently failing after N retries
88. [ ] Introduce centralized error reporting (e.g., Sentry/Application Insights) for unhandled exceptions
89. [ ] Create domain-level invariants and unit tests for Rental and CustomerOrderFlat entities
90. [ ] Add cross-cutting build scripts to run linting, formatting, tests locally with one command
91. [ ] Provide data contracts and schema docs for CustomerOrderFlat (fields, types, constraints) and Rental
92. [ ] Deduplicate configuration bindings by adopting strongly-typed options with named options for each worker
93. [ ] Ensure consistent cultures/format providers for numeric and date parsing/serialization in ETL
94. [ ] Add safeguards for long EF change trackers (use AsNoTracking, clear trackers, or raw bulk APIs as needed)
95. [ ] Set up guarded migrations in production (manual approvals, backup/restore plan)
96. [ ] Add environment-specific logging levels and filters (Development vs Production)
97. [ ] Validate and set command timeouts per DB operation type (extract vs load vs sync)
98. [ ] Provide local development docker-compose for dependent services (MySQL, SQL Server) with seed data
99. [ ] Introduce schema diff checks in CI to detect accidental breaking changes in EF models
100. [ ] Periodically review and refine this checklist; convert completed items into ADRs and documentation
