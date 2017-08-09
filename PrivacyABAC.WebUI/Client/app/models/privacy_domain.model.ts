export class PrivacyDomain {
    public Name: string;
    public FieldsApply: string[];

    constructor(name: string, fieldsApply: string[]) {
        this.Name = name;
        this.FieldsApply = fieldsApply;
    }
}

export class PrivacyDomainFunction {
    public DomainName: string;
    public FunctionName: string;
    public Priority: number;

    constructor(funcName: string, priority: number, domainName: string) {
        this.FunctionName = funcName;
        this.Priority = priority;
        this.DomainName = domainName;
    }
}

export class PrivacyDomainField {
    public DomainName: string;
    public FieldName: string;

    constructor(fieldName: string, domainName: string) {
        this.FieldName = fieldName;
        this.DomainName = domainName;
    }
}

