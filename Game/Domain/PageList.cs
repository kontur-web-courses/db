using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class PageList<T> : List<T>
    {
        [BsonElement]
        public int CurrentPage { get; }
        
        [BsonElement]
        public int TotalPages { get; }
        
        [BsonElement]
        public int PageSize { get; }
        
        [BsonElement]
        public long TotalCount { get; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PageList(List<T> items, long totalCount, int currentPage, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            AddRange(items);
        }
    }
}