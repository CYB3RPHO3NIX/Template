# Copilot Instructions

## Project Guidelines

### Project Structure
- Place domain-related files in Template.Database under Domain/Contexts, Domain/Entities, and Domain/Configurations.
- Preserve subfolder structure for Entities and Configurations.
- Keep generic, non-domain files outside the Domain folder.

### Entity Framework
- Use an easy-to-understand builder-style pattern for table configurations, including columns, foreign keys, relationship types, and support for property names differing from database column names.
- Name EF table-definition classes using "Config" or "Configuration" (e.g., CustomerConfig or CustomerConfiguration) instead of "Map" or "Mapping" to reflect that these classes configure the database tables.