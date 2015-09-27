using System;

namespace Common.Domain
{
    public class AuditData<TId>
    {
        public TId CreateUserId { get; }
        public DateTime UtcCreateTime { get; }
        public TId UpdateUserId { get; }
        public DateTime? UtcUpdateTime { get; }

        public bool WasUpdated => UtcUpdateTime.HasValue;

        protected AuditData()
        { }

        public AuditData(TId createUserId, DateTime utcCreateTime, TId updateUserId, DateTime? utcUpdateTime)
        {
            CreateUserId = createUserId;
            UtcCreateTime = utcCreateTime;
            UpdateUserId = updateUserId;
            UtcUpdateTime = utcUpdateTime;
        }

        public static AuditData<TId> CreateNewAuditData(TId createUserId)
        {
            return new AuditData<TId>(createUserId, DateTime.UtcNow, default(TId), null);
        }

        public AuditData<TId> Update(TId updateUserId)
        {
            return new AuditData<TId>(CreateUserId, UtcCreateTime, updateUserId, DateTime.UtcNow);
        }
    }
}
