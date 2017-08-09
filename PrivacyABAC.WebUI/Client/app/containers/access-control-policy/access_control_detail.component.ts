import { Component } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { SelectItem, Message, ConfirmationService } from 'primeng/primeng';

import { AppSetting } from '../../models/app_setting';
import { AccessControlRule } from '../../models/access_control_rule.model';

@Component({
    selector: 'privacy_rule',
    template: require('./access_control_detail.component.html')
})
export class AccessControlDetailComponent {
    //#region Resource
    private collection_names: SelectItem[] = [];
    private collection_selected_name: string;

    private resource_fields: SelectItem[] = [];
    private resource_selected_field: string;
    private resource_values: string;
    private resource_operators: SelectItem[] = [];
    private resource_selected_operator: string;

    private condition_result: string = "";
    //#endregion

    private policy_id: string = '';
    private description: string = '';

    private actions: SelectItem[] = [];
    private selected_action: string;

    private rule_effects: SelectItem[] = [];
    private selected_rule_effect: string;
    private final_rule_effects: string[] = [];

    private function_names: SelectItem[] = [];
    private selected_function_name: string;

    private subject_fields: SelectItem[] = [];
    private selected_subject_field: string;

    private current_rule_result: string = "";
    private final_rule_result: string[] = [];
    private rules_combining: SelectItem[] = [];
    private selected_rule_combining: string;

    private target_result: string = "";

    private environment_value: string = '';
    private constant_value: string = '';

    private environment_field_options: string[] = ['purpose', 'start_time', 'end_time'];
    private environment_filtered_field: string[];

    private rule_id: string = '';
    private rule_ids: string[] = [];

    private json_helper: any;
    private msgs: Message[] = [];

    private rules: AccessControlRule[] = [];

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private options = new RequestOptions({ headers: this.headers });

    constructor(private http: Http) {
        this.json_helper = JSON;
    }

    ngOnInit() {
        var that = this;
        this.http.get(AppSetting.API_ENDPOINT + 'collections/').subscribe(data => {
            let collections: any[] = data.json();
            for (var name of collections) {
                that.collection_names.push({ label: name, value: name });
            }
            that.collection_selected_name = collections[0];
            that.onSelectCollectionName(collections[0]);
        });
        this.http.get(AppSetting.API_ENDPOINT + 'function/').subscribe(data => {
            let names: any[] = data.json();
            for (var name of names) {
                that.function_names.push({ label: name, value: name });
            }
            that.selected_function_name = names[0];
        });
        this.http.get(AppSetting.API_ENDPOINT + 'subject/fields/').subscribe(data => {
            let jsonObject: any = data.json();
            for (var property in jsonObject) {
                if (property == '_id') continue;
                if (that.selected_subject_field === undefined)
                    that.selected_subject_field = property;
                that.initialize_fields(property, jsonObject, "", that.subject_fields);
            }
        });

        this.actions.push({ label: 'read', value: 'read' });
        this.actions.push({ label: 'create', value: 'create' });
        this.actions.push({ label: 'update', value: 'update' });
        this.actions.push({ label: 'delete', value: 'delete' });
        this.selected_action = this.actions[0].value;

        this.rule_effects.push({ label: 'Permit', value: 'Permit' });
        this.rule_effects.push({ label: 'Deny', value: 'Deny' });
        this.selected_rule_effect = this.rule_effects[0].value;

        this.rules_combining.push({ label: 'Permit overrides', value: 'Permit overrides' });
        this.rules_combining.push({ label: 'Deny overrides', value: 'Deny overrides' });
        this.selected_rule_combining = this.rules_combining[0].value;
    }

    private onSelectCollectionName(collectionSelected: string) {
        var that = this;
        this.resource_fields = [];
        this.http.get(AppSetting.API_ENDPOINT + 'structure/?collectionName=' + collectionSelected).subscribe(data => {
            let jsonObject: any = data.json();
            let initialize_resource_selected: boolean = false;
            for (var property in jsonObject) {
                if (property == '_id') continue;
                if (!initialize_resource_selected) {
                    initialize_resource_selected = true;
                    that.resource_selected_field = property;
                }
                that.initialize_fields(property, jsonObject, "", that.resource_fields);
            }
        })
        this.reset();
    }
    private reset() {
        this.rule_ids = [];
        this.target_result = '';
        this.current_rule_result = '';
        this.rules = [];
    }

    private initialize_fields(property: any, jsonObject: any, prefix: string, container: SelectItem[]) {
        if (property == "_id") return;
        let object = jsonObject[property];
        if (typeof object === 'object' && !Array.isArray(object)) {
            for (var sub_property in object) {
                if (prefix == '')
                    this.initialize_fields(sub_property, object, prefix + property, container);
                else this.initialize_fields(sub_property, object, prefix + '.' + property, container);
            }
        }
        else {
            if (prefix == '')
                container.push({ label: property, value: property });
            else container.push({ label: prefix + '.' + property, value: prefix + '.' + property });
        }
    }

    //#region data form.

    add_function_name_to_rule() {
        this.current_rule_result += this.selected_function_name + " ( ";
    }

    add_function_name_to_target() {
        this.target_result += this.selected_function_name + " ( ";
    }

    add_resource_field_to_rule() {
        this.current_rule_result += "Resource." + this.resource_selected_field + " ";
    }

    add_resource_field_to_target() {
        this.target_result += "Resource." + this.resource_selected_field + " ";
    }

    add_subject_field_to_rule() {
        this.current_rule_result += "Subject." + this.selected_subject_field + " ";
    }

    add_subject_field_to_target() {
        this.target_result += "Subject." + this.selected_subject_field + " ";
    }

    add_constant_value_to_rule() {
        if (this.constant_value == '') {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Constant value can not be null' });
            return;
        }
        if (this.constant_value.indexOf('\'') != -1) {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Constant value can not contain \' character' });
            return;
        }
        this.current_rule_result += "'" + this.constant_value + "' ";
    }

    add_constant_value_to_target() {
        if (this.constant_value == '') {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Constant value can not be null' });
            return;
        }
        if (this.constant_value.indexOf('\'') != -1) {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Constant value can not contain \' character' });
            return;
        }
        this.target_result += "'" + this.constant_value + "' ";
    }

    add_environment_value_to_rule() {
        this.current_rule_result += "Environment." + this.environment_value + " ";
    }

    add_environment_value_to_target() {
        this.target_result += "Environment." + this.environment_value + " ";
    }
    //#endregion

    //#region logic form

    and_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += "AND ";
        } else {
            this.current_rule_result += "AND "
        }
    }

    or_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += "OR ";
        } else {
            this.current_rule_result += "OR "
        }
    }

    not_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += "NOT ( ";
        } else {
            this.current_rule_result += "NOT ( "
        }
    }

    open_bracket_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += "( ";
        } else {
            this.current_rule_result += "( "
        }
    }

    close_bracket_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += ") ";
        } else {
            this.current_rule_result += ") ";
        }
    }

    comma_click(isTarget: boolean) {
        if (isTarget) {
            this.target_result += ", ";
        } else {
            this.current_rule_result += ", ";
        }
    }
    clear_condition(isTarget: boolean) {
        if (isTarget) {
            this.target_result = "";
        } else {
            this.current_rule_result = "";
        }
    }
    //#endregion 

    private add_current_rule() {
        if (this.current_rule_result == '') {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Rule can not be null' });
            return;
        } else if (this.rule_id == '') {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Rule ID can not be null' });
            return;
        }
        for (let r of this.rule_ids) {
            if (r == this.rule_id) {
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Rule ID must be unique' });
                return;
            }
        }
        this.final_rule_result.push(this.current_rule_result);
        this.rule_ids.push(this.rule_id);
        this.final_rule_effects.push(this.selected_rule_effect);
        this.rules.push(new AccessControlRule(this.rule_id, this.current_rule_result, this.selected_rule_effect));
        this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'One rule added' });
    }

    filter_environment_field(event) {
        let query = event.query;
        let filtered: any[] = [];
        for (let i = 0; i < this.environment_field_options.length; i++) {
            let field = this.environment_field_options[i];
            if (field.toLowerCase().indexOf(query.toLowerCase()) == 0) {
                filtered.push(field);
            }
        }
        this.environment_filtered_field = filtered;
    }

    private submit() {
        if (this.policy_id == '') {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Policy ID can not be null' });
            return;
        } else if (this.rules.length == 0) {
            this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Rule can not be null' });
            return;
        }
        let command = {
            "PolicyID": this.policy_id,
            "CollectionName": this.collection_selected_name,
            "Description": this.description,
            "Action": this.selected_action,
            "RuleCombining": this.selected_rule_combining,
            "Target": this.target_result,
            "Rules": this.rules
        };
        let that = this;
        this.http.post(AppSetting.API_ENDPOINT + 'AccessControlPolicy', JSON.stringify(command), this.options).subscribe(
            data => {
                this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'Create Successfully' });
                that.reset();
            },
            error => {
                this.msgs = [];
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: error.text() });
            }
        );
    }
}
