import { SelectItem } from 'primeng/primeng';

export class FieldEffect {
    public Name: string;
    public FunctionApply: string;

    constructor(propertyName: string, privacyFunction: string) {
        this.FunctionApply = privacyFunction;
        this.Name = propertyName;
    }
}

export class FieldEffectOption {
    public Name: string;
    public Functions: SelectItem[];

    constructor(propertyName: string, privacyFunction: SelectItem[]) {
        this.Functions = privacyFunction;
        this.Name = propertyName;
    }
}

export class PrivacyRule {
    public RuleID: string;
    public Condition: string;
    public FieldEffects: FieldEffect[];

    constructor(ruleID: string, condition: string, fieldEffects: FieldEffect[]) {
        this.RuleID = ruleID;
        this.Condition = condition;
        this.FieldEffects = fieldEffects;
    }
}
export class PrivacyPolicy {
    public PolicyID: string;
    public Description: string;
    public CollectionName: string;
    public Target: string;

    constructor(policyID: string, description: string, collectionName: string, target: string) {
        this.PolicyID = policyID;
        this.Description = description;
        this.CollectionName = collectionName;
        this.Target = target;
    }
}