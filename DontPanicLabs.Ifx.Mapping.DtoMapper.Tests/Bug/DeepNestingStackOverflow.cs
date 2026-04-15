namespace AutoMapper.UnitTests.Bug;

// Regression tests for CVE-2026-32933: uncontrolled recursion in TypeMapPlanBuilder
// when building expression trees for cyclic type maps. Without the fix, configuring
// a self-referential map would stack overflow at plan-build time.
public class DeepNestingStackOverflow_DoesNotStackOverflow : AutoMapperSpecBase
{
    public class Node
    {
        public string Value { get; set; }
        public Node Child { get; set; }
    }

    public class NodeDto
    {
        public string Value { get; set; }
        public NodeDto Child { get; set; }
    }

    protected override MapperConfiguration CreateConfiguration() => new(cfg =>
    {
        cfg.CreateMap<Node, NodeDto>();
    });

    [Fact]
    public void Cyclic_map_configuration_does_not_stack_overflow()
    {
        // CreateConfiguration() builds the plan — should not throw StackOverflowException
        Mapper.ShouldNotBeNull();
    }

    [Fact]
    public void Cyclic_map_produces_bounded_depth_output()
    {
        var root = new Node { Value = "root" };
        var current = root;
        for (var i = 0; i < 100; i++)
        {
            current.Child = new Node { Value = $"node-{i}" };
            current = current.Child;
        }

        var result = Mapper.Map<NodeDto>(root);

        result.ShouldNotBeNull();

        int depth = 0;
        var node = result;
        while (node?.Child != null)
        {
            depth++;
            node = node.Child;
        }

        depth.ShouldBeLessThanOrEqualTo(64);
    }
}
