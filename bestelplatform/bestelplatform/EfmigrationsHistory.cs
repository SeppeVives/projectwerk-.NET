using System;
using System.Collections.Generic;

namespace bestelplatform.bestelplatform;

public partial class EfmigrationsHistory
{
    public string MigrationId { get; set; } = null!;

    public string ProductVersion { get; set; } = null!;
}
