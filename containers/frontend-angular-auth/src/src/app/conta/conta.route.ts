import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContaAppComponent } from './conta.app.component';
import { LoginComponent } from './login/login.component';
import { RegistroComponent } from './registro/registro.component';

const contaRouterConfig: Routes = [
    {
        path: '', component: ContaAppComponent,
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'registro', component: RegistroComponent }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(contaRouterConfig)
    ],
    exports: [
        RouterModule
    ]
})
export class ContaRoutingModule { }
