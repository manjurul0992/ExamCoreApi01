using ExamCoreApi01.Models.ViewModels;
using ExamCoreApi01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ExamCoreApi01.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidateDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CandidatesController(CandidateDbContext _context, IWebHostEnvironment _env)
        {
            this._context = _context;
            this._env = _env;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetSkills")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills.ToListAsync();
        }
        [HttpGet]
        [Route("GetCandidate/{id}")]
        public async Task<ActionResult<IEnumerable<CandidateVM>>> GetCandidates(int id)
        {
            List<CandidateVM> candidateSkills = new List<CandidateVM>();
            
            foreach (var candidate in _context.Candidates.ToList())
            {
                var skillList = _context.CandidateSkills.Where(x => x.CandidateId == id).Select(x => new Skill { SkillId = x.SkillId, SkillName = x.Skill.SkillName }).ToList();

                candidateSkills.Add(new CandidateVM
                {
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.CandidateName,
                    BirthDate = candidate.BirthDate,
                    PhoneNo = candidate.PhoneNo,
                    Fresher = candidate.Fresher,
                    Picture = candidate.Picture,
                    SkillList = skillList.ToList()
                });
            }
            var d = candidateSkills.FirstOrDefault(x => x.CandidateId == id);
            if (d != null)
            {
                return Ok(d);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateVM>>> GetCandidateSkills()
        {
            List<CandidateVM> candidateSkills = new List<CandidateVM>();
            var allCandidates = _context.Candidates.ToList();
            foreach (var candidate in allCandidates)
            {
                var skillList = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).Select(x => new Skill { SkillId = x.SkillId,SkillName=x.Skill.SkillName }).ToList();

                candidateSkills.Add(new CandidateVM
                {
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.CandidateName,
                    BirthDate = candidate.BirthDate,
                    PhoneNo = candidate.PhoneNo,
                    Fresher = candidate.Fresher,
                    Picture = candidate.Picture,
                    SkillList = skillList.ToList()
                });
            }
            return candidateSkills;
        }
        
        [HttpPost]
        [Route("PostCanditate")]
        public async Task<ActionResult<CandidateSkill>> PostCandidateSkills([FromForm] CandidateVM VM)
        {
            var skillItems = JsonConvert.DeserializeObject<Skill[]>(VM.SkillStringify);

            Candidate candidate = new Candidate
            {
                CandidateName = VM.CandidateName,
                BirthDate = VM.BirthDate,
                PhoneNo = VM.PhoneNo,
                Fresher = VM.Fresher
            };

            if (VM.PictureFile != null)
            {
                var webroot = _env.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(VM.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await VM.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }

            foreach (var item in skillItems)
            {
                var candidateskill = new CandidateSkill
                {
                    Candidate = candidate,
                    CandidateId = candidate.CandidateId,
                    SkillId = item.SkillId,
                    //if
                    //SkillName = item.SkillName
                   
                   
                };
                _context.Add(candidateskill);
            }

            await _context.SaveChangesAsync();
            return Ok(candidate);
        }
        [Route("Update")]
        [HttpPut]
        public async Task<ActionResult<CandidateSkill>> UpdateBookingEntry([FromForm] CandidateVM vm)
        {
            var skillItems = JsonConvert.DeserializeObject<Skill[]>(vm.SkillStringify);

            Candidate candidate = _context.Candidates.Find(vm.CandidateId);
            candidate.CandidateName = vm.CandidateName;
            candidate.BirthDate = vm.BirthDate;
            candidate.PhoneNo = vm.PhoneNo;
            candidate.Fresher = vm.Fresher;

            if (vm.PictureFile != null)
            {
                var webroot = _env.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await vm.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }

       
            var existingSkills = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _context.CandidateSkills.Remove(item);
            }

          
            foreach (var item in skillItems)
            {
                var candidateSkill = new CandidateSkill
                {
                    CandidateId = candidate.CandidateId,
                    SkillId = item.SkillId,
                    //if
                   // SkillName = item.SkillName
                };
                _context.Add(candidateSkill);
            }

            _context.Entry(candidate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(candidate);
        }
        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult<CandidateSkill>> DeleteCandidateSkill(int id)
        {
            Candidate candidate = _context.Candidates.Find(id);

            var existingSkills = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _context.CandidateSkills.Remove(item);
            }
            _context.Entry(candidate).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            return Ok(candidate);
        }
    }

}
