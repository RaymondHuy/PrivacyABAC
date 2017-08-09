import { Component, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { SelectItem, Message, ConfirmationService } from 'primeng/primeng';

import { AppSetting } from '../../models/app_setting';
import { PrivacyDomain, PrivacyDomainFunction, PrivacyDomainField } from '../../models/privacy_domain.model';

@Component({
    selector: 'privacy_domain',
    template: require('./privacy_domain_form_create.component.html'),
    providers: [ConfirmationService]
})

export class PrivacyDomainFormCreateComponent {

    private configured_domain_names: SelectItem[] = [];
    private configured_domain_selected_name: string;

    private collection_names: SelectItem[] = [];
    private collection_selected_name: string;

    private resource_fields: SelectItem[] = [];
    private resource_selected_field: string;

    private configured_privacy_domain_functions: PrivacyDomainFunction[] = [];
    private configured_privacy_domain_functions_view: PrivacyDomainFunction[] = [];
    private configured_privacy_domain_fields: PrivacyDomainField[] = [];
    private configured_privacy_domain_fields_view: PrivacyDomainField[] = [];

    private domain_name: string;
    private function_name: string = '';
    private priority_function: number = 1;

    private json_helper: any;
    private msgs: Message[] = [];

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private options = new RequestOptions({ headers: this.headers });

    constructor(private http: Http) {
        this.json_helper = JSON;
    }

    ngOnInit() {
        let that = this;

        this.http.get(AppSetting.API_ENDPOINT + 'collections/').subscribe(data => {
            let collections: any[] = data.json();
            for (var name of collections) {
                that.collection_names.push({ label: name, value: name });
            }
            that.collection_selected_name = collections[0];
            that.onSelectCollectionName(collections[0]);
        });
        this.initialize_domains();
    }
    initialize_domains() {
        this.configured_domain_names = [];
        this.configured_privacy_domain_functions = [];
        this.configured_privacy_domain_fields = [];
        let that = this;
        this.http.get(AppSetting.API_ENDPOINT + 'PrivacyDomainField/').subscribe(data => {
            let collections: any[] = data.json();
            for (var domain of collections) {
                that.configured_domain_names.push({ label: domain.domainName, value: domain.domainName });
                for (var func of domain.functions) {
                    that.configured_privacy_domain_functions.push(new PrivacyDomainFunction(func.name, func.priority, domain.domainName));
                }
                for (var field of domain.fields) {
                    that.configured_privacy_domain_fields.push(new PrivacyDomainField(field, domain.domainName));
                }
            }
            that.configured_domain_selected_name = that.configured_domain_names[0].label;
            that.onSelectDomainName(that.configured_domain_selected_name);
        });
    }
    onSelectDomainName(domain_selected: string) {
        this.configured_privacy_domain_functions_view = this.configured_privacy_domain_functions.filter(x => x.DomainName == domain_selected);
        this.configured_privacy_domain_fields_view = this.configured_privacy_domain_fields.filter(x => x.DomainName == domain_selected);
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
                that.initialize_field_effects(property, jsonObject, "", that.resource_fields);
            }
        });
    }
    private initialize_field_effects(property: any, jsonObject: any, prefix: string, container: SelectItem[]) {
        if (property == "_id") return;
        let that = this;
        let object = jsonObject[property];
        if (typeof object === 'object' && !Array.isArray(object)) {
            for (var sub_property in object) {
                if (prefix == '')
                    this.initialize_field_effects(sub_property, object, prefix + property, container);
                else this.initialize_field_effects(sub_property, object, prefix + '.' + property, container);
            }
        }
        else {
            if (prefix == '') {
                container.push({ label: property, value: property });
            }
            else {
                container.push({ label: prefix + '.' + property, value: prefix + '.' + property });
            }
        }
    }
    private updatePriorityFunctions() {
        let priority_functions: any[] = [];
        for (let func of this.configured_privacy_domain_functions_view) {
            priority_functions.push({ "Name": func.FunctionName, "Priority": func.Priority });
        }
        let command = {
            "DomainName": this.configured_domain_selected_name,
            "PriorityFunctions": priority_functions
        };
        this.http.post(AppSetting.API_ENDPOINT + 'PriorityFunctions', JSON.stringify(command), this.options).subscribe(
            data => {
                this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'Update Priority Successfully' });
            },
            error => {
                this.msgs = [];
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: error });
            }
        );
    }
    private add_function() {
        let command = {
            "DomainName": this.configured_domain_selected_name,
            "Priority": { "Name": this.function_name, "Priority": this.priority_function } 
        };
        this.http.post(AppSetting.API_ENDPOINT + 'PrivacyDomainFunction', JSON.stringify(command), this.options).subscribe(
            data => {
                this.initialize_domains();
                this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'Function Added Successfully' });
            },
            error => {
                this.msgs = [];
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: error });
            }
        );
    }
    private addField() {
        let fieldName = this.collection_selected_name + "." + this.resource_selected_field;
        for (let field of this.configured_privacy_domain_fields_view) {
            if (field.FieldName == fieldName) {
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: 'Field already existed' });
                return;
            }
        }
        let command = {
            "DomainName": this.configured_domain_selected_name,
            "FieldName": fieldName
        };
        this.http.post(AppSetting.API_ENDPOINT + 'DomainField', JSON.stringify(command), this.options).subscribe(
            data => {
                this.initialize_domains();
                this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'Field Added Successfully' });
            },
            error => {
                this.msgs = [];
                this.msgs.push({ severity: 'error', summary: 'Error Message', detail: error });
            }
        );
    }
    private addDomain() {
        let name: string = this.domain_name;
        this.http.post(AppSetting.API_ENDPOINT + 'PrivacyDomain', JSON.stringify(name), this.options).subscribe(
            data => {
                this.initialize_domains();
                this.msgs.push({ severity: 'info', summary: 'Info Message', detail: 'Insert Domain Successfully' });

            });
    }
}
