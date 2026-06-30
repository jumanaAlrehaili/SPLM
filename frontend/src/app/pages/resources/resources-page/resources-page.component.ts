import { Component } from '@angular/core';
import { ResourceListComponent } from '../resource-list/resource-list.component';

@Component({
  selector: 'app-resources-page',
  standalone: true,
  imports: [ResourceListComponent],
  templateUrl: './resources-page.component.html',
  styleUrl: './resources-page.component.scss'
})
export class ResourcesPageComponent {}
