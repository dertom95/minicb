using BuildingLogic;

public class ChildProduction : JobLogicBase {
    public override bool NeedsToGoBackToOwner() {
        return false;
    }
}