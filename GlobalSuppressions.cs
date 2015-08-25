
#define CODE_ANALYSIS

using System.Diagnostics.CodeAnalysis;

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Language", "CSE0001:Use nameof when passing parameter names as arguments", Justification = "Using C# 5", Scope = "module")]
[assembly: SuppressMessage("Language", "CSE0002:Use getter-only auto properties", Justification = "Using C# 5", Scope = "module")]
[assembly: SuppressMessage("Language", "CSE0003:Use expression-bodied members", Justification = "Using C# 5", Scope = "module")]

[assembly: SuppressMessage("Potential Code Quality Issues", "RECS0105:Type check and casts can be replaced with 'as' and null check", Justification = "_", Scope = "module")]
[assembly: SuppressMessage("Potential Code Quality Issues", "RECS0133:Parameter name differs in base declaration", Justification = "This one is fine.", Scope = "module")]
[assembly: SuppressMessage("Potential Code Quality Issues", "RECS0143:Cannot resolve symbol in text argument", Justification = "_", Scope = "module")]
[assembly: SuppressMessage("Potential Code Quality Issues", "RECS0163:Suggest the usage of the nameof operator", Justification = "Using C# 5", Scope = "module")]
