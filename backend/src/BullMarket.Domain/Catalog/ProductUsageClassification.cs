public sealed record ProductUsageClassification(
        bool IsAiReady,
        bool IsLocalLlmReady,
        bool IsDeveloperReady,
        bool IsCreatorReady,
        bool IsGamingReady,
        bool IsDockerReady);