using Microsoft.AspNetCore.Mvc;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
namespace testSOAP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SOAPController : ControllerBase
    {

        [HttpGet("getall")]

        public string Test()
        {
            //patient data model
            Patient patient = new Patient
            {
                Id = "12345",

                Name = new List<HumanName>
              {
                  new HumanName
                  {
                      Family = "Doe",
                      Given = new List<string> { "John" }
                  }
              },
                BirthDate = "1980-01-01",
                Active = true,
                Gender = AdministrativeGender.Male,
                Telecom = new List<ContactPoint>
                {
                    {new ContactPoint
                        {
                            System = ContactPoint.ContactPointSystem.Phone,
                            Value = "555-1234"
                        }
                    },
                },
                GeneralPractitioner = new List<ResourceReference>
                {
                    new ResourceReference
                    {
                        Reference = "Practitioner/doc123"
                    }
                },
                ManagingOrganization = new ResourceReference
                {
                    Reference = "Organization/org123"
                },
               

            };
            // doctor or nurse or assistant data model
            Practitioner doctor = new Practitioner
            {
                Id = "doc123",
                Active = true,
                Name = new List<HumanName>
                {
                    {
                        new HumanName{ Family = "Smith",
                        Given = new List<string> { "Jane" } }
                    }
                },
                Address= new List<Address>
                {
                    new Address
                    {
                        Line = new List<string>{ "456 Elm St" },
                    },
                },Gender = AdministrativeGender.Male,
                Qualification=new List<Practitioner.QualificationComponent>
                {
                    new Practitioner.QualificationComponent
                    {
                        Code = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://terminology.hl7.org/CodeSystem/v2-0360/2.7",
                                    Code = "MD",
                                    Display = "Doctor of Medicine"
                                }
                            }
                        }
                    }
                },
               
            };
            // hospital or clinic data model
            Organization hospital = new Organization
            {
                Id = "org123",
                Name = "Example Healthcare Organization",
                Telecom = new List<ContactPoint>
            {
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Phone,
                    Value = "123-456-7890"
                }
            },
                Address = new List<Address>
            {
                new Address
                {
                    Line = new List<string> { "123 Main St" },
                    City = "Anytown",
                    State = "CA",
                    PostalCode = "12345"
                }
                },
                Type = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/organization-type",
                                Code = "prov",
                                Display = "Healthcare Provider"
                            }
                        }
                    }
                },
                Active = true
            };
            // doctor - nurse - assistant role data model 
            PractitionerRole doctor_role = new PractitionerRole

            {
                Id = "role123",
                Active = true,
                Practitioner = new ResourceReference
                {
                    Reference = $"Practitioner/{doctor.Id}"
                },
                Organization = new ResourceReference
                {
                    Reference = $"Organization/{hospital.Id}"
                },
                Code = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://terminology.hl7.org/CodeSystem/practitioner-role",
                            Code = "doctor",
                            Display = "Doctor"
                        }
                    }
                }
            },
                Location = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Reference = $"Location/{hospital.Id}"
                }
            },
            };
            // appointment data model
            Appointment appointment = new Appointment
            {
                Id = "app123",
                Status = Appointment.AppointmentStatus.Booked,
                Start = DateTimeOffset.Now,
                End = DateTimeOffset.Now.AddHours(1)
            };
            // encounter data model when patient arrives at hospital 
            Encounter encounter = new Encounter
            {
                Id = "enc123",
                Status = Encounter.EncounterStatus.InProgress,
                Class = new Coding
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                    Code = "AMB"
                },
                Subject = new ResourceReference
                {
                    Reference = $"Patient/{patient.Id}"
                },
                Appointment = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Reference = $"Appointment/{appointment.Id}"
                }
            },
                Location = new List<Encounter.LocationComponent>
            {
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Reference = $"Organization/{hospital.Id}"
                    }
                }
            }
            };
            // condition for the patient when added to the database
            Condition condition = new Condition
            {
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div>Patient has a history of hypertension.</div>"
                },
                Subject = new ResourceReference
                {
                    Reference = $"Patient/{patient.Id}"
                },
            };

            #region soap note
            // soap note data model
            Composition soapNote = new Composition
            {
                Status =CompositionStatus.Final,
                Type = new CodeableConcept("http://loinc.org", "11488-4", "SOAP note"),
                Subject = new ResourceReference("Patient/123", "John Doe"),
                DateElement = new FhirDateTime(DateTime.UtcNow),
                Author = new List<ResourceReference>
            {
                new ResourceReference("Practitioner/456", "Dr. Ali Mohammed")
            },
                Title = "SOAP Note for John Doe"
            };

            // Sections: Subjective, Objective, Assessment, Plan
            soapNote.Section = new List<Composition.SectionComponent>
        {
            new Composition.SectionComponent
            {
                Title = "Subjective",
                Code = new CodeableConcept("http://loinc.org", "10164-2", "History of Present Illness"),
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Patient reports chest pain for 3 days.</div>"
                }
            },
            new Composition.SectionComponent
            {
                Title = "Objective",
                Code = new CodeableConcept("http://loinc.org", "29545-1", "Physical findings"),
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div xmlns='http://www.w3.org/1999/xhtml'>BP 140/90, HR 88, ECG abnormal.</div>"
                }
            },
            new Composition.SectionComponent
            {
                Title = "Assessment",
                Code = new CodeableConcept("http://loinc.org", "51848-0", "Assessment note"),
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Possible angina.</div>"
                }
            },
            new Composition.SectionComponent
            {
                Title = "Plan",
                Code = new CodeableConcept("http://loinc.org", "18776-5", "Treatment plan"),
                Text = new Narrative
                {
                    Status = Narrative.NarrativeStatus.Generated,
                    Div = "<div xmlns='http://www.w3.org/1999/xhtml'>Schedule stress test, start aspirin.</div>"
                }
            }
        };
            #endregion
            return patient.ToJson() + "\n" +
                   hospital.ToJson() + "\n" +
                   appointment.ToJson() + "\n" +
                   encounter.ToJson() + "\n" +
                   soapNote.ToJson();

        }
    }
}

