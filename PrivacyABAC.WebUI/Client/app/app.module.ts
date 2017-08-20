import { NgModule, Inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule, APP_BASE_HREF } from '@angular/common';
import { HttpModule, Http } from '@angular/http';
import { FormsModule } from '@angular/forms';

import {
    ButtonModule, GrowlModule, DropdownModule, AutoCompleteModule, InputTextModule, DataTableModule,
    SharedModule, InputTextareaModule, MessagesModule, PanelModule, AccordionModule, FieldsetModule, ConfirmDialogModule
} from 'primeng/primeng';

// i18n support
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './containers/home/home.component';
import { UsersComponent } from './containers/users/users.component';
import { UserDetailComponent } from './components/user-detail/user-detail.component';
import { AccessControlPolicyFormCreateComponent } from './containers/access-control-policy/access_control_form_create.component';
import { AccessControlDetailComponent } from './containers/access-control-policy/access_control_detail.component';
import { PrivacyDomainFormCreateComponent } from './containers/privacy-policy/privacy_domain_form_create.component';
import { PrivacyPolicyDetailComponent } from './containers/privacy-policy/privacy_policy_detail.component';
import { PrivacyPolicyFormCreateComponent } from './containers/privacy-policy/privacy_policy_form_create.component';
import { SubPrivacyPolicyFormCreateComponent } from './containers/privacy-policy/sub_privacy_policy_form_create.component';
import { PrivacyCheckingComponent } from './containers/privacy-checking/privacy_checking.component';

import { NotFoundComponent } from './containers/not-found/not-found.component';

import { LinkService } from './shared/link.service';
import { UserService } from './shared/user.service';
import { ConnectionResolver } from './shared/route.resolver';
import { ORIGIN_URL } from './shared/constants/baseurl.constants';
import { TransferHttpModule } from '../modules/transfer-http/transfer-http.module';

export function createTranslateLoader(http: Http, baseHref) {
    // Temporary Azure hack
    if (baseHref === null && typeof window !== 'undefined') {
        baseHref = window.location.origin;
    }
    // i18n files are in `wwwroot/assets/`
    return new TranslateHttpLoader(http, `${baseHref}/assets/i18n/`, '.json');
}

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        UsersComponent,
        UserDetailComponent,
        HomeComponent,
        AccessControlPolicyFormCreateComponent,
        AccessControlDetailComponent,
        PrivacyDomainFormCreateComponent,
        PrivacyPolicyDetailComponent,
        PrivacyPolicyFormCreateComponent,
        SubPrivacyPolicyFormCreateComponent,
        PrivacyCheckingComponent,
        NotFoundComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,

        ButtonModule,
        GrowlModule,
        DropdownModule,
        AutoCompleteModule, InputTextareaModule, MessagesModule, AccordionModule,
        InputTextModule, DataTableModule, SharedModule, PanelModule, FieldsetModule, ConfirmDialogModule,
          
        TransferHttpModule, // Our Http TransferData method

        // i18n support
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: (createTranslateLoader),
                deps: [Http, [ORIGIN_URL]]
            }
        }),

        // App Routing
        RouterModule.forRoot([
            {
                path: '',
                redirectTo: 'home',
                pathMatch: 'full'
            },
            {
                path: 'home', component: HomeComponent
            },
            { path: 'privacy_checking', component: PrivacyCheckingComponent },
            //{ path: 'policy_review', component: PolicyReviewComponent },
            { path: 'access_control_policy', component: AccessControlPolicyFormCreateComponent },
            { path: 'access_control_detail/:id', component: AccessControlDetailComponent },
            { path: 'privacy_policy', component: PrivacyPolicyFormCreateComponent },
            { path: 'privacy_policy_detail/:id', component: PrivacyPolicyDetailComponent },
            { path: 'sub_privacy_policy', component: SubPrivacyPolicyFormCreateComponent },
            { path: 'privacy_domain', component: PrivacyDomainFormCreateComponent },
            //{ path: 'policy_management', component: PolicyManagementComponent },
            {
                path: '**', component: NotFoundComponent
            }
        ])
    ],
    providers: [
        LinkService,
        UserService,
        ConnectionResolver,
        TranslateModule
    ]
})
export class AppModule {
}
