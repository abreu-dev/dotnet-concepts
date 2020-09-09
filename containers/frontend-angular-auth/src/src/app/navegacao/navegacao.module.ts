import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { FooterComponent } from './footer/footer.component';
import { MenuLoginComponent } from './menu-login/menu-login.component';
import { MenuComponent } from './menu/menu.component';


@NgModule({
    declarations: [
        MenuComponent,
        MenuLoginComponent,
        FooterComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        NgbModule
    ],
    exports: [
        MenuComponent,
        MenuLoginComponent,
        FooterComponent
    ]
})
export class NavegacaoModule { }
