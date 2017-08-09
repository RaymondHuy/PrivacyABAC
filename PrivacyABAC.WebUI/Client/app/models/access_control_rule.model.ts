export class AccessControlRule {
    public RuleId: string;
    public Condition: string;
    public Effect: string;

    constructor(ruleId: string, condition: string, effect: string) {
        this.RuleId = ruleId;
        this.Condition = condition;
        this.Effect = effect;
    }
}

export class AccessControl {
    public PolicyID: string;
    public Description: string;
    public CollectionName: string;
    public RuleCombining: string;
    public Action: string;
    public Target: string;

    constructor(policyID: string, description: string, collectionName: string, ruleCombining: string, target: string, action:string="read") {
        this.PolicyID = policyID;
        this.CollectionName = collectionName;
        this.Description = description;
        this.RuleCombining = ruleCombining;
        this.Action = action;
        this.Target = target;
    }
}