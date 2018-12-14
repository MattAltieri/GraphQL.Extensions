using GraphQL.Extensions.Pagination;
using GraphQL.Extensions.Test;
using System.Collections.Generic;
using Xunit;
using Xunit2.Should;

namespace System.Linq {
    public class IQueryableSlicerExtensionsTest {
        
        [Theory]
        [MemberData(nameof(GetSliceTestData))]
        public void Should_ReturnSlicedResult_When_BasicSliceCalled(Slicer<MockEntity> slicer, IQueryable<MockEntity> testData,
            IQueryable<MockEntity> expectedResults) {

            IQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = testData.Slice(slicer));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            results.SequenceEqual(expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetSliceBeforeTestData))]
        public void Should_ReturnSlicedResult_When_SliceBeforeCalled(BeforeSlicer<MockEntity> slicer,
            IQueryable<MockEntity> testData, IQueryable<MockEntity> expectedResults) {

            IQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = testData.Slice(slicer));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            results.SequenceEqual(expectedResults).ShouldBeTrue();
        }

        public static List<object[]> GetSliceTestData
            => new List<object[]> {
                new object[] {
                    new Slicer<MockEntity> {
                        OrderBy = new OrderByInfo<MockEntity>("Id", SortDirections.Ascending,
                            new ThenByInfo<MockEntity>("Name", SortDirections.Ascending,
                            new ThenByInfo<MockEntity>("DOB", SortDirections.Ascending))),
                        First = 10,
                    },
                    TestHelpers.TestData_TripleSort.AsQueryable(),
                    TestHelpers.TestData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id).ThenBy(o => o.Name).ThenBy(o => o.DOB)
                        .Take(10)
                },
            };
        
        public static List<object[]> GetSliceBeforeTestData
            => new List<object[]> {
                new object[] {
                    new BeforeSlicer<MockEntity> {
                        OrderBy = new OrderByInfo<MockEntity>("Id", SortDirections.Ascending,
                            new ThenByInfo<MockEntity>("Name", SortDirections.Ascending,
                            new ThenByInfo<MockEntity>("DOB", SortDirections.Ascending))),
                        First = 10,
                        Before = "id::a::2//name::a::C//dob::a::625541184000000000",
                    },
                    TestHelpers.TestData_TripleSort.AsQueryable(),
                    TestHelpers.TestData_TripleSort.AsQueryable()
                            .OrderBy(o => o.Id).ThenBy(o => o.Name).ThenBy(o => o.DOB)
                            .Where(o => o.Id > 2)
                            .Where(o => "C".CompareTo(o.Name) > 0)
                            .Where(o => o.DOB > new DateTime(625541184000000000))
                            .Take(10)
                },
            };
    }
}